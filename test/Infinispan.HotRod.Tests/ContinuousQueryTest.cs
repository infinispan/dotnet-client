using System;
using System.Collections.Generic;
using System.Linq;
using Infinispan.HotRod.Exceptions;
using Infinispan.HotRod.Config;
using Org.Infinispan.Query.Remote.Client;
using System.IO;
using Org.Infinispan.Protostream;
using SampleBankAccount;
using NUnit.Framework;
using System.Threading;
using Infinispan.HotRod.Protobuf;
using System.Collections.ObjectModel;

/**
 * Known issues: HRCPP-301, HRCPP-302
 * 
 * No queries use sorting because of HRCPP-301.
 * No queries use pagination as JPQL itself does not support it.
 * 
 */
namespace Infinispan.HotRod.Tests.ClusteredIndexingXml
{
    class ProtobufMarshaller : BasicTypesProtoStreamMarshaller
    {
        override public object ObjectFromByteBuffer(byte[] buf)
        {
            WrappedMessage wm = WrappedMessage.Parser.ParseFrom(buf);
            object o;
            switch (wm.ScalarOrMessageCase)
            {
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedDouble:
                    o = wm.WrappedDouble;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedFloat:
                    o = wm.WrappedFloat;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedInt64:
                    o = wm.WrappedInt64;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedUInt64:
                    o = wm.WrappedUInt64;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedInt32:
                    o = wm.WrappedInt32;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedFixed64:
                    o = wm.WrappedFixed64;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedFixed32:
                    o = wm.WrappedFixed32;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedBool:
                    o = wm.WrappedBool;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedString:
                    o = wm.WrappedString;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedBytes:
                    o = wm.WrappedBytes;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedUInt32:
                    o = wm.WrappedUInt32;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedSFixed32:
                    o = wm.WrappedSFixed32;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedSFixed64:
                    o = wm.WrappedSFixed64;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedSInt32:
                    o = wm.WrappedSInt32;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedSInt64:
                    o = wm.WrappedSInt64;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedDescriptorFullName:
                    o = wm.WrappedDescriptorFullName;
                    break;
                case WrappedMessage.ScalarOrMessageOneofCase.WrappedMessageBytes:
                    switch (wm.WrappedDescriptorId)
                    {
                        case 1000042:
                            o = User.Parser.ParseFrom(wm.WrappedMessageBytes);
                            break;
                        case 1000044:
                            o = Account.Parser.ParseFrom(wm.WrappedMessageBytes);
                            break;
                        case 1000045:
                            o = Transaction.Parser.ParseFrom(wm.WrappedMessageBytes);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return o;

        }
    }
    [TestFixture]
    [Category("clustered_indexing_xml")]
    [Category("RemoteQueryTestSuite")]
    class ContinuousQueryTest
    {
        RemoteCacheManager remoteManager;
        const String ERRORS_KEY_SUFFIX = ".errors";
        const String PROTOBUF_METADATA_CACHE_NAME = "___protobuf_metadata";
        const String NAMED_CACHE = "CacheForQueryTest";

        [OneTimeSetUp]
        public void BeforeClass()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ProtocolVersion("2.6");
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            conf.Marshaller(new ProtobufMarshaller());
            remoteManager = new RemoteCacheManager(conf.Build(), true);

            IRemoteCache<String, String> metadataCache = remoteManager.GetCache<String, String>(PROTOBUF_METADATA_CACHE_NAME);
            metadataCache.Remove(ERRORS_KEY_SUFFIX);
            metadataCache.Put("sample_bank_account/bank.proto", File.ReadAllText("proto2/bank.proto"));
            if (metadataCache.ContainsKey(ERRORS_KEY_SUFFIX))
            {
                Assert.Fail("fail: error in registering .proto model");
            }

            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);
        }
        [OneTimeTearDown]
        public void AfterClass()
        {
            remoteManager.Stop();
        }
        [Test]
        public void EntityBasicContQueryTest()
        {
            int joined = 0, updated = 0, leaved = 0;
            try
            {
                IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);
                userCache.Clear();
                Semaphore s = new Semaphore(0, 1);
                QueryRequest qr = new QueryRequest();
                // JpqlString will be deprecated please use QueryString
                // qr.JpqlString = "from sample_bank_account.User";
                qr.QueryString = "from sample_bank_account.User";

                Event.ContinuousQueryListener<int, User> cql = new Event.ContinuousQueryListener<int, User>(qr.QueryString);
                cql.JoiningCallback = (int k, User v) => { joined++; };
                cql.LeavingCallback = (int k, User v) => { leaved++; s.Release(); };
                cql.UpdatedCallback = (int k, User v) => { updated++; };
                userCache.AddContinuousQueryListener(cql);

                User u = CreateUser1(userCache);
                userCache.Put(1, u);
                u.Name = "Jerry";
                u.Surname = "Mouse";
                userCache.Put(1, u);
                userCache.Remove(1);
                s.WaitOne(10000);
                userCache.RemoveContinuousQueryListener(cql);
                userCache.Clear();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Assert.AreEqual(1, joined);
            Assert.AreEqual(1, updated);
            Assert.AreEqual(1, leaved);
        }
        [Test]
        public void ProjectionBasicContQueryTest()
        {
            int joined = 0, updated = 0, leaved = 0;
            object[] uT = null, lT = null;
            try
            {
                IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);
                userCache.Clear();
                Semaphore s = new Semaphore(0, 1);
                QueryRequest qr = new QueryRequest();
                qr.QueryString = "select id, name from sample_bank_account.User";
                Event.ContinuousQueryListener<int, object[]> cql = new Event.ContinuousQueryListener<int, object[]>(qr.QueryString);
                cql.JoiningCallback = (int k, object[] v) => { joined++; };
                cql.UpdatedCallback = (int k, object[] v) => {
                    uT = v;
                    updated++;
                };
                cql.LeavingCallback = (int k, object[] v) => {
                    lT = v;
                    leaved++;
                    s.Release(); };

                userCache.AddContinuousQueryListener(cql);

                User u1 = CreateUser1(userCache);
                User u2 = CreateUser2(userCache);

                userCache.Put(1, u1);
                userCache.Put(2, u2);
                u1.Name = "Jerry";
                u1.Surname = "Mouse";
                userCache.Put(1, u1);
                userCache.Remove(2);
                s.WaitOne(10000);
                userCache.RemoveContinuousQueryListener(cql);
                userCache.Clear();
                remoteManager.Stop();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Assert.AreEqual(2, joined);
            Assert.AreEqual(1, updated);
            Assert.AreEqual(1, leaved);
            Assert.AreEqual(uT, new object[] { 1, "Jerry"});
            Assert.AreEqual(lT, new object[] { 2, "Spider"});
        }

        private User CreateUser1(IRemoteCache<int, User> remoteCache)
        {
            User user1 = new User();
            user1.Id = 1;
            user1.Name = "Tom";
            user1.Surname = "Cat";
            user1.Gender = User.Types.Gender.Female;
            User.Types.Address address1 = new User.Types.Address();
            address1.Street = "Via Roma";
            address1.PostCode = "202020";
            address1.Number = 3;
            List<User.Types.Address> addresses = new List<User.Types.Address>();
            addresses.Add(address1);
            user1.Addresses.Add(addresses);
            return user1;
        }

        private User CreateUser2(IRemoteCache<int, User> remoteCache)
        {
            User user2 = new User();
            user2.Id = 2;
            user2.Name = "Spider";
            user2.Surname = "Man";
            user2.Gender = User.Types.Gender.Male;
            List<Int32> accountIds = new List<Int32>();
            accountIds.Add(3);
            user2.AccountIds.Add(accountIds);
            User.Types.Address address2 = new User.Types.Address();
            address2.Street = "Old Street";
            address2.PostCode = "Y12";
            address2.Number = -12;
            User.Types.Address address3 = new User.Types.Address();
            address3.Street = "Bond Street";
            address3.PostCode = "ZZ";
            address3.Number = 312;
            List<User.Types.Address> addresses = new List<User.Types.Address>();
            addresses.Add(address2);
            addresses.Add(address3);
            user2.Addresses.Add(addresses);
            return user2;
        }
        private User PutUser3(IRemoteCache<int, User> remoteCache)
        {
            User user3 = new User();
            user3.Id = 3;
            user3.Name = "Spider";
            user3.Surname = "Woman";
            user3.Gender = User.Types.Gender.Female;

            remoteCache.Put(3, user3);
            List<Int32> accountIds = new List<Int32>();
            user3.AccountIds.Add(accountIds);
            return user3;
        }


        private void PutAccounts(IRemoteCache<int, Account> remoteCache)
        {
            Account account1 = new Account();
            account1.Id = 1;
            account1.Description = "John Doe's first bank account";
            account1.CreationDate = MakeDate("2013-01-03");

            remoteCache.Put(4, account1);

            Account account2 = new Account();
            account2.Id = 2;
            account2.Description = "John Doe's second bank account";
            account2.CreationDate = MakeDate("2013-01-04");

            remoteCache.Put(5, account2);

            Account account3 = new Account();
            account3.Id = 3;
            account3.CreationDate = MakeDate("2013-01-20");

            remoteCache.Put(6, account3);
        }

        private void PutTransactions(IRemoteCache<int, Transaction> remoteCache)
        {
            Transaction transaction0 = new Transaction();
            transaction0.Id = 0;
            transaction0.Description = "Birthday present";
            transaction0.AccountId = 1;
            transaction0.Amount = 1800;
            transaction0.Date = MakeDate("2012-09-07");
            transaction0.IsDebit = false;
            transaction0.IsValid = true;

            remoteCache.Put(7, transaction0);

            Transaction transaction1 = new Transaction();
            transaction1.Id = 1;
            transaction1.Description = "Feb. rent payment";
            transaction1.AccountId = 1;
            transaction1.Amount = 1500;
            transaction1.Date = MakeDate("2013-01-05");
            transaction1.IsDebit = true;
            transaction1.IsValid = true;

            remoteCache.Put(8, transaction1);

            Transaction transaction2 = new Transaction();
            transaction2.Id = 2;
            transaction2.Description = "Starbucks";
            transaction2.AccountId = 1;
            transaction2.Amount = 23;
            transaction2.Date = MakeDate("2013-01-09");
            transaction2.IsDebit = true;
            transaction2.IsValid = true;

            remoteCache.Put(9, transaction2);

            Transaction transaction3 = new Transaction();
            transaction3.Id = 3;
            transaction3.Description = "Hotel";
            transaction3.AccountId = 2;
            transaction3.Amount = 45;
            transaction3.Date = MakeDate("2013-02-27");
            transaction3.IsDebit = true;
            transaction3.IsValid = true;

            remoteCache.Put(10, transaction3);

            Transaction transaction4 = new Transaction();
            transaction4.Id = 4;
            transaction4.Description = "Last january";
            transaction4.AccountId = 2;
            transaction4.Amount = 95;
            transaction4.Date = MakeDate("2013-01-31");
            transaction4.IsDebit = true;
            transaction4.IsValid = true;

            remoteCache.Put(11, transaction4);

            Transaction transaction5 = new Transaction();
            transaction5.Id = 5;
            transaction5.Description = "-Popcorn";
            transaction5.AccountId = 2;
            transaction5.Amount = 4;
            transaction5.Date = MakeDate("2013-01-01");
            transaction5.IsDebit = true;
            transaction5.IsValid = true;

            remoteCache.Put(12, transaction5);
        }

        private ulong MakeDate(String date)
        {
            //For compatibility with Java side, use the number of milliseconds since 
            //January 1, 1970, 00:00:00. The time zone is not taken into account
            //in this example.
            DateTime inception = new DateTime(1970, 1, 1, 0, 0, 0);
            DateTime current = DateTime.Parse(date);
            return (ulong)current.Subtract(inception).TotalMilliseconds;
        }
    }
}
