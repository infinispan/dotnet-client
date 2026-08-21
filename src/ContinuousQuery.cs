using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Infinispan.Hotrod
{
    public enum CQResultType : byte
    {
        Joining = 1,
        Updated = 2,
        Leaving = 3
    }

    public class CQEvent
    {
        public CQResultType Type { get; internal set; }
        public byte[] Key { get; internal set; }
        public byte[] Value { get; internal set; }
        public IList<byte[]> Projections { get; internal set; }
    }

    public class CQResult
    {
        public CQResultType ResultType;
        public byte[] Key;
        public byte[] Value;
        public List<byte[]> Projections;
    }

    public class ContinuousQuery : IAsyncDisposable
    {
        private readonly CacheBase _cache;
        private readonly InfinispanClient _client;
        private readonly CQListener _listener;
        private readonly Channel<CQEvent> _channel;

        public ChannelReader<CQEvent> Events => _channel.Reader;

        internal ContinuousQuery(InfinispanClient client, CacheBase cache, string query,
            IDictionary<string, object> namedParams = null, int channelSize = 64)
        {
            _client = client;
            _cache = cache;
            _channel = Channel.CreateBounded<CQEvent>(new BoundedChannelOptions(channelSize)
            {
                FullMode = BoundedChannelFullMode.Wait
            });
            _listener = new CQListener(_channel.Writer);

            var filterParams = BuildCQParams(query, namedParams);

            var cmd = new Commands.ADDCLIENTLISTENER
            {
                Listener = _listener,
                IncludeState = 1,
                FilterFactoryName = "continuous-query-filter-converter-factory",
                FilterParams = filterParams,
                ConverterFactoryName = "continuous-query-filter-converter-factory",
                ConverterParams = filterParams,
                Interests = 0x0F,
                isBinary = true
            };

            _client.AddListener(cache, _listener, true, cmd).GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            await _client.RemoveListener(_cache, _listener);
            _channel.Writer.TryComplete();
        }

        private static byte[][] BuildCQParams(string query, IDictionary<string, object> namedParams)
        {
            var result = new List<byte[]> { WrappedMessageHelper.WrapString(query) };
            if (namedParams != null)
            {
                foreach (var kv in namedParams)
                {
                    result.Add(WrappedMessageHelper.WrapString(kv.Key));
                    result.Add(kv.Value switch
                    {
                        string s => WrappedMessageHelper.WrapString(s),
                        int i => WrappedMessageHelper.WrapInt32(i),
                        long l => WrappedMessageHelper.WrapInt64(l),
                        _ => WrappedMessageHelper.WrapString(kv.Value.ToString())
                    });
                }
            }
            return result.ToArray();
        }
    }

    internal class CQListener : AbstractClientListener
    {
        private readonly ChannelWriter<CQEvent> _writer;
        private string _listenerId;

        public CQListener(ChannelWriter<CQEvent> writer)
        {
            _writer = writer;
            var bytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            _listenerId = Convert.ToHexString(bytes).ToLowerInvariant();
        }

        public override string ListenerID
        {
            get => _listenerId;
            set => _listenerId = value;
        }

        public override void OnEvent(Event e)
        {
            if (e.CustomMarker != 0 && e.customData != null)
            {
                var inner = WrappedMessageHelper.UnwrapBytes(e.customData);
                if (inner == null) return;
                var result = WrappedMessageHelper.DecodeCQResult(inner);
                var ev = new CQEvent
                {
                    Type = result.ResultType,
                    Key = result.Key,
                    Value = result.Value,
                    Projections = result.Projections
                };
                _writer.TryWrite(ev);
            }
        }

        public override void OnError(Exception ex)
        {
            _writer.TryComplete(ex);
        }
    }
}
