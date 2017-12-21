using System;
using NUnit.Framework;
using Infinispan.HotRod.Config;

namespace Infinispan.HotRod.Tests
{
    public abstract class BaseAuthorizationTest
    {
        public const string HOTROD_HOST = "127.0.0.1";
        public const int HOTROD_PORT = 11222;
        public const string AUTH_CACHE = "authCache";
        public const string PROTOBUF_SCRIPT_CACHE_NAME = "___script_cache";
        public const string REALM = "ApplicationRealm";

        private IRemoteCache<String, String> readerCache;
        private IRemoteCache<String, String> writerCache;
        private IRemoteCache<String, String> supervisorCache;
        private IRemoteCache<String, String> adminCache;
        private IRemoteCache<String, String> scriptCache;

        private AuthorizationTester tester = new AuthorizationTester();

        IMarshaller marshaller;

        public abstract string GetMech();

        [TestFixtureSetUp]
        public void BeforeClass()
        {
            readerCache = InitCache("reader", "password");
            writerCache = InitCache("writer", "somePassword");
            supervisorCache = InitCache("supervisor", "lessStrongPassword");
            adminCache = InitCache("admin", "strongPassword");
            scriptCache = InitCache("admin", "strongPassword", PROTOBUF_SCRIPT_CACHE_NAME);
        }

        private IRemoteCache<String, String> InitCache(string user, string password, string cacheName = AUTH_CACHE)
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer()
                    .Host(HOTROD_HOST)
                    .Port(HOTROD_PORT)
                    .ConnectionTimeout(90000)
                    .SocketTimeout(900);
            AuthenticationConfigurationBuilder authBuilder = conf.Security().Authentication();
            authBuilder.Enable()
                       .SaslMechanism(GetMech())
                       .ServerFQDN("node0")
                       .SetupCallback(user, password, REALM);
            marshaller = new JBasicMarshaller();
            conf.Marshaller(marshaller);
            Configuration c = conf.Build();
            RemoteCacheManager remoteManager = new RemoteCacheManager(c, true);
            IRemoteCache<string, string> cache = remoteManager.GetCache<string, string>(cacheName);
            return cache;
        }

        [Test]
        public void ReaderSuccessTest()
        {
            tester.TestReaderSuccess(readerCache);
        }

        [Test]
        public void ReaderPerformsWritesTest()
        {
            tester.TestReaderPerformsWrites(readerCache);
        }

        [Test]
        public void WriterSuccessTest()
        {
            tester.TestWriterSuccess(writerCache);
        }

        [Test]
        public void WriterPerformsReadsTest()
        {
            tester.TestWriterPerformsReads(writerCache);
        }

        [Test]
        public void WriterPerformsSupervisorOpsTest()
        {
            tester.TestWriterPerformsSupervisorOps(writerCache, scriptCache, marshaller);
        }

        [Test]
        public void SupervisorSuccessTest()
        {
            tester.TestSupervisorSuccess(supervisorCache, scriptCache, marshaller);
        }

        [Test]
        public void SupervisorPerformsAdminOpsTest()
        {
            tester.TestSupervisorPerformsAdminOps(supervisorCache);
        }

        [Test]
        public void AdminSuccessTest()
        {
            tester.TestAdminSuccess(adminCache, scriptCache, marshaller);
        }
    }
}
