using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CqTest;
using Google.Protobuf;
using Infinispan.Hotrod;
using Infinispan.Hotrod.Tests.Util;
using Org.Infinispan.Protostream;
using Xunit;

namespace Infinispan.Hotrod.XUnitTest
{
    internal class CQPersonMarshaller : Marshaller<object>
    {
        public override byte[] marshall(object obj)
        {
            if (obj is string s)
            {
                int t = CodedOutputStream.ComputeTagSize(9);
                int sz = CodedOutputStream.ComputeStringSize(s);
                byte[] bytes = new byte[t + sz];
                var cos = new CodedOutputStream(bytes);
                cos.WriteTag((9 << 3) + 2);
                cos.WriteString(s);
                cos.Flush();
                return bytes;
            }
            if (obj is CQPerson person)
            {
                int size = person.CalculateSize();
                byte[] personBytes = new byte[size];
                var cos = new CodedOutputStream(personBytes);
                person.WriteTo(cos);
                cos.Flush();
                return WrappedMessageHelper.WrapMessage(personBytes, "cq_test.CQPerson");
            }
            throw new NotSupportedException($"Cannot marshall {obj?.GetType()}");
        }

        public override object unmarshall(byte[] buff)
        {
            var wm = WrappedMessage.Parser.ParseFrom(buff);
            if (wm.ScalarOrMessageCase == WrappedMessage.ScalarOrMessageOneofCase.WrappedString)
                return wm.WrappedString;
            if (wm.ScalarOrMessageCase == WrappedMessage.ScalarOrMessageOneofCase.WrappedMessageBytes)
                return CQPerson.Parser.ParseFrom(wm.WrappedMessageBytes);
            return buff;
        }
    }

    public class ContinuousQueryTestFixture : IAsyncLifetime
    {
        private InfinispanContainer _container;
        public InfinispanClient infinispan;
        public Cache<object, object> cache;

        public async Task InitializeAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string protoDef;
            using (var stream = assembly.GetManifestResourceStream("Infinispan.Hotrod.XUnitTest.resources.proto2.cq_person.proto"))
            using (var reader = new StreamReader(stream))
            {
                protoDef = reader.ReadToEnd();
            }

            _container = new InfinispanContainer("infinispan-cq.xml", "admin", "password");
            await _container.StartAsync();

            infinispan = new InfinispanClient();
            infinispan.User = "admin";
            infinispan.Password = "password";
            infinispan.AuthMech = "SCRAM-SHA-256";
            infinispan.AddHost(_container.Host, _container.Port);
            infinispan.Version = ProtocolVersion.Version31;
            infinispan.ClientIntelligence = ClientIntelligence.Basic;

            var metaCache = infinispan.NewCache(new StringMarshaller(), new StringMarshaller(), "___protobuf_metadata");
            var kvMediaType = new MediaType
            {
                CustomMediaType = Encoding.ASCII.GetBytes("text/plain"),
                InfoType = 2
            };
            metaCache.KeyMediaType = kvMediaType;
            metaCache.ValueMediaType = kvMediaType;
            await metaCache.Remove(".errors");
            await metaCache.Put("cq_person.proto", protoDef);
            if (await metaCache.ContainsKey(".errors"))
            {
                var errors = await metaCache.Get(".errors");
                throw new Exception($"Proto registration failed: {errors}");
            }

            cache = infinispan.NewCache(new CQPersonMarshaller(), new CQPersonMarshaller(), "cq-test");
            var protoMt = new MediaType
            {
                CustomMediaType = Encoding.ASCII.GetBytes("application/x-protostream"),
                InfoType = 2
            };
            cache.KeyMediaType = protoMt;
            cache.ValueMediaType = protoMt;
            await cache.Clear();
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }

    [Collection("MainSequence")]
    public class ContinuousQueryTest : IClassFixture<ContinuousQueryTestFixture>
    {
        private readonly Cache<object, object> _cache;

        public ContinuousQueryTest(ContinuousQueryTestFixture fixture)
        {
            _cache = fixture.cache;
        }

        [Fact]
        public async Task JoiningEventTest()
        {
            await _cache.Clear();
            await using var cq = _cache.ContinuousQuery("FROM cq_test.CQPerson WHERE age >= 18");

            await _cache.Put("alice", new CQPerson { Name = "Alice", Age = 25 });

            var ev = await ReadEventWithTimeout(cq, TimeSpan.FromSeconds(10));
            Assert.NotNull(ev);
            Assert.Equal(CQResultType.Joining, ev.Type);
        }

        [Fact]
        public async Task LeavingEventTest()
        {
            await _cache.Clear();
            await using var cq = _cache.ContinuousQuery("FROM cq_test.CQPerson WHERE age >= 18");

            await _cache.Put("bob", new CQPerson { Name = "Bob", Age = 30 });
            var ev1 = await ReadEventWithTimeout(cq, TimeSpan.FromSeconds(10));
            Assert.NotNull(ev1);
            Assert.Equal(CQResultType.Joining, ev1.Type);

            await _cache.Put("bob", new CQPerson { Name = "Bob", Age = 10 });
            var ev2 = await ReadEventWithTimeout(cq, TimeSpan.FromSeconds(10));
            Assert.NotNull(ev2);
            Assert.Equal(CQResultType.Leaving, ev2.Type);
        }

        [Fact]
        public async Task NonMatchingEntryNoEventTest()
        {
            await _cache.Clear();
            await using var cq = _cache.ContinuousQuery("FROM cq_test.CQPerson WHERE age >= 18");

            await _cache.Put("child", new CQPerson { Name = "Child", Age = 10 });

            var ev = await ReadEventWithTimeout(cq, TimeSpan.FromSeconds(3));
            Assert.Null(ev);
        }

        private static async Task<CQEvent> ReadEventWithTimeout(ContinuousQuery cq, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            try
            {
                if (await cq.Events.WaitToReadAsync(cts.Token))
                {
                    if (cq.Events.TryRead(out var ev))
                        return ev;
                }
            }
            catch (OperationCanceledException) { }
            return null;
        }
    }
}
