using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infinispan.Hotrod
{
    public class InfinispanHost : IDisposable
    {
        public InfinispanHost(InfinispanClient cluster, string host, int port)
        {
            Name = host;
            Port = port;
            Cluster = cluster;
            SSL = Cluster.UseTLS;
            Available = true;
        }

        private int _disposed;
        private long _messageId;
        private InfinispanConnection _connection;
        private ResponseStream _responseStream;
        private readonly SemaphoreSlim _connectLock = new(1, 1);
        private readonly SemaphoreSlim _writeLock = new(1, 1);
        private Task _readLoopTask;
        private readonly ConcurrentDictionary<long, PendingRequest> _pending = new();
        internal readonly ConcurrentDictionary<string, IClientListener> Listeners = new();

        public readonly InfinispanClient Cluster;
        public string Name { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public string User { get; set; }
        public string Domain { get; set; }
        public string AuthMech { get; set; }
        public bool SSL { get; set; }
        public bool Available { get; set; }

        public long NewMessageId()
        {
            return Interlocked.Increment(ref _messageId);
        }

        public async Task<Result> ExecuteAsync(CacheBase cache, Command cmd)
        {
            var conn = await EnsureConnectedAsync();
            if (conn == null)
            {
                return new Result { ResultType = ResultType.NetError, Messge = "Connection failed" };
            }

            var messageId = NewMessageId();
            var context = new CommandContext
            {
                MessageId = messageId,
                Client = conn,
                Cache = cache
            };
            var tcs = new TaskCompletionSource<Result>(TaskCreationOptions.RunContinuationsAsynchronously);
            var pending = new PendingRequest { Command = cmd, Context = context, Cache = cache, Completion = tcs };
            _pending[messageId] = pending;

            try
            {
                await _writeLock.WaitAsync();
                try
                {
                    conn.Send(context, cmd);
                }
                finally
                {
                    _writeLock.Release();
                }
            }
            catch (Exception ex)
            {
                _pending.TryRemove(messageId, out _);
                return new Result { ResultType = ResultType.NetError, Messge = ex.Message };
            }

            return await tcs.Task;
        }

        private async Task<InfinispanConnection> EnsureConnectedAsync()
        {
            if (_connection?.IsConnected == true)
                return _connection;

            await _connectLock.WaitAsync();
            try
            {
                if (_connection?.IsConnected == true)
                    return _connection;

                _connection?.Disconnect();
                _connection = new InfinispanConnection(this);
                if (!await _connection.ConnectAsync())
                {
                    Available = false;
                    return null;
                }
                Available = true;

                if (!string.IsNullOrEmpty(Password))
                {
                    if (!await AuthenticateAsync(_connection))
                        return null;
                }

                _responseStream = new ResponseStream(_connection.GetStream());
                _readLoopTask = Task.Run(ReadLoop);
                return _connection;
            }
            finally
            {
                _connectLock.Release();
            }
        }

        private async Task<bool> AuthenticateAsync(InfinispanConnection conn)
        {
            var rs = new ResponseStream(conn.GetStream());

            var mechList = new Commands.AUTHMECHLIST();
            var ctx = new CommandContext { MessageId = NewMessageId(), Client = conn, Cache = null, VersionOverride = InfinispanClient.HANDSHAKE_VERSION };
            conn.Send(ctx, mechList);
            if (!ReadSingleResponse(rs, ctx.MessageId, out var status))
                return false;
            var req = new InfinispanRequest { ResponseStatus = status, Cluster = Cluster, Client = conn };
            mechList.OnReceive(req, rs);

            bool found = false;
            if (AuthMech != null)
            {
                foreach (var mech in mechList.availableMechs)
                {
                    if (AuthMech.Equals(mech))
                    {
                        found = true;
                        break;
                    }
                }
            }
            if (!found)
                return false;

            var auth = new Commands.AUTH(AuthMech, new System.Net.NetworkCredential(User, Password, Domain));
            while (auth.Completed == 0)
            {
                ctx = new CommandContext { MessageId = NewMessageId(), Client = conn, Cache = null, VersionOverride = InfinispanClient.HANDSHAKE_VERSION };
                conn.Send(ctx, auth);
                if (!ReadSingleResponse(rs, ctx.MessageId, out status))
                    return false;
                req = new InfinispanRequest { ResponseStatus = status, Cluster = Cluster, Client = conn };
                auth.OnReceive(req, rs);
            }
            return true;
        }

        private bool ReadSingleResponse(ResponseStream rs, long expectedMessageId, out byte status)
        {
            status = 0;
            if (rs.ReadByte() != 0xA1)
                return false;
            var inMessageId = Codec.readVLong(rs);
            if (inMessageId != 0 && inMessageId != expectedMessageId)
                return false;
            rs.ReadByte(); // opcode
            status = (byte)rs.ReadByte();
            var topologyChanged = (byte)rs.ReadByte();
            if (topologyChanged != 0)
                ReadAndApplyTopology(rs, null);
            if (Codec30.hasError(status))
            {
                Codec.readArray(rs); // consume error message
                return false;
            }
            return true;
        }

        private void ReadLoop()
        {
            try
            {
                while (_disposed == 0)
                {
                    var magic = _responseStream.ReadByte();
                    if (magic != 0xA1)
                    {
                        FailAllPending("Bad Magic Number");
                        break;
                    }
                    var messageId = Codec.readVLong(_responseStream);
                    var opCode = (byte)_responseStream.ReadByte();
                    var status = (byte)_responseStream.ReadByte();
                    var topologyChanged = (byte)_responseStream.ReadByte();

                    if (topologyChanged != 0)
                    {
                        // Find any pending request's cache for topology context
                        CacheBase topologyCache = null;
                        foreach (var p in _pending.Values)
                        {
                            if (p.Cache != null) { topologyCache = p.Cache; break; }
                        }
                        ReadAndApplyTopology(_responseStream, topologyCache);
                    }

                    if (IsEvent(opCode))
                    {
                        DispatchEvent(opCode);
                        continue;
                    }

                    var errMsg = Codec30.hasError(status) ? Codec.readArray(_responseStream) : null;

                    if (_pending.TryRemove(messageId, out var pending))
                    {
                        if (errMsg != null)
                        {
                            pending.Completion.TrySetResult(new Result
                            {
                                ResultType = ResultType.Error,
                                Messge = Encoding.ASCII.GetString(errMsg)
                            });
                        }
                        else
                        {
                            var request = new InfinispanRequest
                            {
                                ResponseStatus = status,
                                ResponseOpCode = opCode,
                                Cluster = Cluster,
                                Client = _connection,
                                Command = pending.Command,
                                context = pending.Context
                            };
                            try
                            {
                                var result = pending.Command.OnReceive(request, _responseStream);
                                pending.Completion.TrySetResult(result);
                            }
                            catch (Exception ex)
                            {
                                pending.Completion.TrySetResult(new Result
                                {
                                    ResultType = ResultType.Error,
                                    Messge = ex.Message
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_disposed == 0)
                    FailAllPending(ex.Message);
            }
        }

        private bool IsEvent(byte opCode)
        {
            return Enum.IsDefined(typeof(EventType), opCode);
        }

        private void DispatchEvent(byte opCode)
        {
            var listenerId = StringMarshaller._ASCII.unmarshall(Codec.readArray(_responseStream));
            var e = new Event
            {
                ListenerID = listenerId,
                CustomMarker = (byte)_responseStream.ReadByte(),
                Retried = (byte)_responseStream.ReadByte()
            };
            if (e.CustomMarker == 0)
            {
                e.Key = Codec.readArray(_responseStream);
                e.Type = (EventType)opCode;
                if (e.Type != EventType.REMOVED && e.Type != EventType.EXPIRED)
                    e.Version = Codec.readLong(_responseStream);
            }
            else
            {
                e.customData = Codec.readArray(_responseStream);
            }

            if (Listeners.TryGetValue(listenerId, out var listener))
                Task.Run(() => listener.OnEvent(e));
        }

        private void ReadAndApplyTopology(ResponseStream rs, CacheBase cache)
        {
            var t = new TopologyInfo { TopologyId = Codec.readVUInt(rs) };
            var serversNum = Codec.readVInt(rs);
            t.servers = new List<Tuple<byte[], ushort>>();
            t.hosts = new InfinispanHost[serversNum];
            for (int i = 0; i < serversNum; i++)
            {
                var addr = Codec.readArray(rs);
                var port = Codec.readUnsignedShort(rs);
                t.servers.Add(Tuple.Create(addr, port));
            }
            t.HashFuncNum = (byte)rs.ReadByte();
            if (t.HashFuncNum > 0)
            {
                var segmentsNum = Codec.readVInt(rs);
                t.OwnersPerSegment = new List<List<int>>();
                for (int i = 0; i < segmentsNum; i++)
                {
                    var ownerNumPerSeg = (byte)rs.ReadByte();
                    var owners = new List<int>();
                    for (int j = 0; j < ownerNumPerSeg; j++)
                        owners.Add(Codec.readVInt(rs));
                    t.OwnersPerSegment.Add(owners);
                }
            }

            if (cache != null && Monitor.TryEnter(Cluster.mActiveCluster))
            {
                try { Cluster.UpdateTopologyInfo(t, cache); }
                finally { Monitor.Exit(Cluster.mActiveCluster); }
            }
        }

        private void FailAllPending(string message)
        {
            foreach (var key in _pending.Keys)
            {
                if (_pending.TryRemove(key, out var pending))
                {
                    pending.Completion.TrySetResult(new Result
                    {
                        ResultType = ResultType.NetError,
                        Messge = message
                    });
                }
            }
            foreach (var listener in Listeners.Values)
            {
                Task.Run(() => listener.OnError(new IOException(message)));
            }
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
            {
                FailAllPending("Host disposed");
                _connection?.Disconnect();
            }
        }
    }

    internal class PendingRequest
    {
        public Command Command;
        public CommandContext Context;
        public CacheBase Cache;
        public TaskCompletionSource<Result> Completion;
    }
}
