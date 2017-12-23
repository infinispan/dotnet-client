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

/**
 * No queries use pagination as JPQL itself does not support it.
 * 
 */
namespace Infinispan.HotRod.Tests.ClusteredIndexingXml
{
    [TestFixture]
    [Category("clustered_indexing_xml")]
    class RemoteQueryTest
    {
        RemoteCacheManager remoteManager;
        const String ERRORS_KEY_SUFFIX = ".errors";
        const String PROTOBUF_METADATA_CACHE_NAME = "___protobuf_metadata";
        const String NAMED_CACHE = "InMemoryNonSharedIndex";

        [OneTimeSetUp]
        public void BeforeClass()
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222);
            conf.ConnectionTimeout(90000).SocketTimeout(6000);
            conf.Marshaller(new BasicTypesProtoStreamMarshaller());
            remoteManager = new RemoteCacheManager(conf.Build(), true);

            IRemoteCache<String, String> metadataCache = remoteManager.GetCache<String, String>(PROTOBUF_METADATA_CACHE_NAME);
            metadataCache.Put("sample_bank_account/bank.proto", File.ReadAllText("proto2/bank.proto"));
            if (metadataCache.ContainsKey(ERRORS_KEY_SUFFIX))
            {
                Console.WriteLine("fail: error in registering .proto model");
                Environment.Exit(-1);
            }

            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);
            userCache.Clear();
            PutUsers(userCache);
            IRemoteCache<int, Account> accountCache = remoteManager.GetCache<int, Account>(NAMED_CACHE);
            PutAccounts(accountCache);
            IRemoteCache<int, Transaction> transactionCache = remoteManager.GetCache<int, Transaction>(NAMED_CACHE);
            PutTransactions(transactionCache);
        }

        [Test]
        public void GetAllTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User";
            qr.QueryString = "from sample_bank_account.User";

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(3, listOfUsers.Count);
        }

        [Test]
        public void Eq1Test()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User u where u.name = \"John\"";
            qr.QueryString = "from sample_bank_account.User u where u.name = \"John\"";

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(1, listOfUsers.Count);
            Assert.AreEqual("John", listOfUsers.ElementAt(0).Name);
            Assert.AreEqual("Doe", listOfUsers.ElementAt(0).Surname);
        }

        [Test]
        public void EqEmptyStringTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User u where u.name = \"\"";
            qr.QueryString = "from sample_bank_account.User u where u.name = \"\"";

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(0, listOfUsers.Count);
        }

        [Test]
        public void EqSentenceTest()
        {
            IRemoteCache<int, Account> accountCache = remoteManager.GetCache<int, Account>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.Account a where a.description = \"John Doe's first bank account\"";
            qr.QueryString = "from sample_bank_account.Account a where a.description = \"John Doe's first bank account\"";

            QueryResponse result = accountCache.Query(qr);
            List<User> listOfAccounts = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(1, listOfAccounts.Count);
            Assert.AreEqual(1, listOfAccounts.ElementAt(0).Id);
        }

        [Test]
        public void EqNonIndexedFieldTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User u where u.notes = \"Lorem ipsum dolor sit amet\"";
            qr.QueryString = "from sample_bank_account.User u where u.notes = \"Lorem ipsum dolor sit amet\"";

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(1, listOfUsers.Count);
            Assert.AreEqual(1, listOfUsers.ElementAt(0).Id);
        }

        [Test]
        public void EqHybridQueryWithParamTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);
            
            QueryRequest.Types.NamedParameter param = new QueryRequest.Types.NamedParameter();
            WrappedMessage wm = new WrappedMessage();
            wm.WrappedString = "Doe";
            param.Name = "surnameParam";
            param.Value = wm;

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User u where (u.notes = \"Lorem ipsum dolor sit amet\") and (u.surname = :surnameParam)";
            qr.QueryString = "from sample_bank_account.User u where (u.notes = \"Lorem ipsum dolor sit amet\") and (u.surname = :surnameParam)";
            qr.NamedParameters.Add(param);

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(1, listOfUsers.Count);
            Assert.AreEqual(1, listOfUsers.ElementAt(0).Id);
        }

        [Test]
        public void EqInNested1Test()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User u where u.addresses.postCode = \"X1234\"";
            qr.QueryString = "from sample_bank_account.User u where u.addresses.postCode = \"X1234\"";

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(1, listOfUsers.Count);
            Assert.AreEqual("X1234", listOfUsers.ElementAt(0).Addresses.ElementAt(0).PostCode);
        }

        [Test]
        public void LikeTest()
        {
            IRemoteCache<int, Transaction> txCache = remoteManager.GetCache<int, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.Transaction t where t.description like \"%rent%\"";
            qr.QueryString = "from sample_bank_account.Transaction t where t.description like \"%rent%\"";
            QueryResponse result = txCache.Query(qr);
            List<Transaction> listOfTx = RemoteQueryUtils.unwrapResults<Transaction>(result);

            Assert.AreEqual(1, listOfTx.Count);
            Assert.AreEqual(1, listOfTx.ElementAt(0).AccountId);
            Assert.AreEqual(1500, listOfTx.ElementAt(0).Amount, 0);
        }

        [Test]
        public void BetweenTest()
        {
            IRemoteCache<int, Transaction> txCache = remoteManager.GetCache<int, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.Transaction t where t.date between \""+ MakeDate("2013-01-01") +"\" and \"" + MakeDate("2013-01-31") + "\"";
            qr.QueryString = "from sample_bank_account.Transaction t where t.date between \""+ MakeDate("2013-01-01") +"\" and \"" + MakeDate("2013-01-31") + "\"";
            QueryResponse result = txCache.Query(qr);
            List<Transaction> listOfTx = RemoteQueryUtils.unwrapResults<Transaction>(result);

            Assert.AreEqual(4, listOfTx.Count);
            foreach (Transaction tx in listOfTx)
            {
                Assert.True(tx.Date >= MakeDate("2013-01-01") && tx.Date <= MakeDate("2013-01-31"));
            }
        }

        [Test]
        public void GreaterThanTest()
        {
            IRemoteCache<int, Transaction> txCache = remoteManager.GetCache<int, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.Transaction t where t.amount > 1500";
            qr.QueryString = "from sample_bank_account.Transaction t where t.amount > 1500";
            QueryResponse result = txCache.Query(qr);
            List<Transaction> listOfTx = RemoteQueryUtils.unwrapResults<Transaction>(result);

            Assert.AreEqual(1, listOfTx.Count);
            Assert.True(listOfTx.ElementAt(0).Amount > 1500);
        }

        [Test]
        public void OrTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User u where (u.surname = \"Man\") or (u.surname = \"Woman\")";
            qr.QueryString = "from sample_bank_account.User u where (u.surname = \"Man\") or (u.surname = \"Woman\")";

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(2, listOfUsers.Count);
            foreach (User u in listOfUsers)
            {
                Assert.AreEqual("Spider", u.Name);      
            }
        }

        [Test]
        public void NotTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User u where u.name != \"Spider\"";
            qr.QueryString = "from sample_bank_account.User u where u.name != \"Spider\"";

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(1, listOfUsers.Count);
            Assert.AreEqual("John", listOfUsers.ElementAt(0).Name);
        }

        [Test]
        public void InvalidEmbeddedAttributeTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "select u.addresses from sample_bank_account.User u";
            qr.QueryString = "select u.addresses from sample_bank_account.User u";

            Assert.Throws<HotRodClientException>(() => userCache.Query(qr));
        }

        [Test]
        public void RejectProjectionOfRepeatedPropertyTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "select u.addresses.postcode from sample_bank_account.User u";
            qr.QueryString = "select u.addresses.postcode from sample_bank_account.User u";

            Assert.Throws<HotRodClientException>(() => userCache.Query(qr));
        }

        [Test]
        public void ProjectionTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "select u.name, u.surname, u.age from sample_bank_account.User u where u.age is null";
            qr.QueryString = "select u.name, u.surname, u.age from sample_bank_account.User u where u.age is null";

            QueryResponse result = userCache.Query(qr);
            List<Object[]> projections = RemoteQueryUtils.unwrapWithProjection(result);
            Assert.AreEqual("Spider", projections.ElementAt(0)[0]);
            Assert.AreEqual("Man", projections.ElementAt(0)[1]);
            Assert.AreEqual("Spider", projections.ElementAt(1)[0]);
            Assert.AreEqual("Woman", projections.ElementAt(1)[1]);
        }

        [Test]
        public void ContainsTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User u where u.accountIds = 2";
            qr.QueryString = "from sample_bank_account.User u where u.accountIds = 2";

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(1, listOfUsers.Count);
            Assert.AreEqual("John", listOfUsers.ElementAt(0).Name);
        }

        [Test]
        public void ContainsAllTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User u where u.accountIds = 1 and u.accountIds = 2" ;
            qr.QueryString = "from sample_bank_account.User u where u.accountIds = 1 and u.accountIds = 2" ;

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(1, listOfUsers.Count);
            Assert.AreEqual(1, listOfUsers.ElementAt(0).Id);
        }

        [Test]
        public void NotContainsAllTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User u where u.accountIds = 1 and u.accountIds = 2 and u.accountIds = 3";
            qr.QueryString = "from sample_bank_account.User u where u.accountIds = 1 and u.accountIds = 2 and u.accountIds = 3";

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(0, listOfUsers.Count);
        }

        [Test]
        public void NotContainsTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User u where u.accountIds = 42";
            qr.QueryString = "from sample_bank_account.User u where u.accountIds = 42";

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(0, listOfUsers.Count);
        }

        [Test]
        public void ContainsAnyTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User u where u.accountIds = 2 or u.accountIds = 3 order by u.id asc";
            qr.QueryString = "from sample_bank_account.User u where u.accountIds = 2 or u.accountIds = 3 order by u.id asc";

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(2, listOfUsers.Count);
            Assert.AreEqual(1, listOfUsers.ElementAt(0).Id);
            Assert.AreEqual(2, listOfUsers.ElementAt(1).Id);
        }

        [Test]
        public void NotContainsAnyTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User u where u.accountIds = 4 or u.accountIds = 5";
            qr.QueryString = "from sample_bank_account.User u where u.accountIds = 4 or u.accountIds = 5";

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(0, listOfUsers.Count);
        }

        [Test]
        public void InTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User u where u.id in (1, 3)";
            qr.QueryString = "from sample_bank_account.User u where u.id in (1, 3)";

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(2, listOfUsers.Count);
        }

        [Test]
        public void NotInTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.User u where u.id in (4)";
            qr.QueryString = "from sample_bank_account.User u where u.id in (4)";

            QueryResponse result = userCache.Query(qr);
            List<User> listOfUsers = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(0, listOfUsers.Count);
        }

        [Test]
        public void StringLiteralEscapeTest()
        {
            IRemoteCache<int, Account> userCache = remoteManager.GetCache<int, Account>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "from sample_bank_account.Account a where a.description = 'John Doe''s first bank account'";
            qr.QueryString = "from sample_bank_account.Account a where a.description = 'John Doe''s first bank account'";

            QueryResponse result = userCache.Query(qr);
            List<User> listOfAccounts = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(1, listOfAccounts.Count);
            Assert.AreEqual(1, listOfAccounts.ElementAt(0).Id);
        }

        [Test]
        public void HavingWithSumTest()
        {
            IRemoteCache<int, Transaction> userCache = remoteManager.GetCache<int, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "select t.accountId, sum(t.amount) from sample_bank_account.Transaction t group by t.accountId having sum(t.amount) > 3300";
            qr.QueryString = "select t.accountId, sum(t.amount) from sample_bank_account.Transaction t group by t.accountId having sum(t.amount) > 3300";

            QueryResponse result = userCache.Query(qr);
            
            List<Object[]> projections = RemoteQueryUtils.unwrapWithProjection(result);
           
            Assert.AreEqual(1, projections.Count);
            Assert.AreEqual(1, projections.ElementAt(0)[0]);
            Assert.AreEqual(3323.0, (double) projections.ElementAt(0)[1], 0.001d);
        }

        [Test]
        public void HavingWithAvgTest()
        {
            IRemoteCache<int, Transaction> userCache = remoteManager.GetCache<int, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "select t.accountId, avg(t.amount) from sample_bank_account.Transaction t group by t.accountId having avg(t.amount) < 100";
            qr.QueryString = "select t.accountId, avg(t.amount) from sample_bank_account.Transaction t group by t.accountId having avg(t.amount) < 100";

            QueryResponse result = userCache.Query(qr);

            List<Object[]> projections = RemoteQueryUtils.unwrapWithProjection(result);

            Assert.AreEqual(1, projections.Count);
            Assert.AreEqual(2, projections.ElementAt(0)[0]);
            Assert.AreEqual(48.0, (double)projections.ElementAt(0)[1], 0.001d);
        }

        [Test]
        public void HavingWithMinTest()
        {
            IRemoteCache<int, Transaction> userCache = remoteManager.GetCache<int, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "select t.accountId, min(t.amount) from sample_bank_account.Transaction t group by t.accountId having min(t.amount) < 10";
            qr.QueryString = "select t.accountId, min(t.amount) from sample_bank_account.Transaction t group by t.accountId having min(t.amount) < 10";

            QueryResponse result = userCache.Query(qr);

            List<Object[]> projections = RemoteQueryUtils.unwrapWithProjection(result);

            Assert.AreEqual(1, projections.Count);
            Assert.AreEqual(2, projections.ElementAt(0)[0]);
            Assert.AreEqual(4.0, (double)projections.ElementAt(0)[1], 0.001d);
        }

        [Test]
        public void HavingWithMaxTest()
        {
            IRemoteCache<int, Transaction> userCache = remoteManager.GetCache<int, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "select t.accountId, max(t.amount) from sample_bank_account.Transaction t group by t.accountId having max(t.amount) > 1000";
            qr.QueryString = "select t.accountId, max(t.amount) from sample_bank_account.Transaction t group by t.accountId having max(t.amount) > 1000";

            QueryResponse result = userCache.Query(qr);

            List<Object[]> projections = RemoteQueryUtils.unwrapWithProjection(result);

            Assert.AreEqual(1, projections.Count);
            Assert.AreEqual(1, projections.ElementAt(0)[0]);
            Assert.AreEqual(1800.0, (double)projections.ElementAt(0)[1], 0.001d);
        }

        [Test]
        public void GlobalSumTest()
        {
            IRemoteCache<int, Transaction> userCache = remoteManager.GetCache<int, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "select sum(t.amount) from sample_bank_account.Transaction t";
            qr.QueryString = "select sum(t.amount) from sample_bank_account.Transaction t";

            QueryResponse result = userCache.Query(qr);

            List<Object[]> projections = RemoteQueryUtils.unwrapWithProjection(result);

            Assert.AreEqual(1, projections.Count);
            Assert.AreEqual(3467.0, (double)projections.ElementAt(0)[0], 0.001d);
        }

        [Test]
        public void CountTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            // JpqlString will be deprecated please use QueryString
	    // qr.JpqlString = "select u.name, count(u.age) from sample_bank_account.User u group by u.name";
            qr.QueryString = "select u.name, count(u.age) from sample_bank_account.User u group by u.name";

            QueryResponse result = userCache.Query(qr);

            List<Object[]> projections = RemoteQueryUtils.unwrapWithProjection(result);

            Assert.AreEqual(2, projections.Count);
            Assert.AreEqual("John" ,projections.ElementAt(0)[0]);
            Assert.AreEqual(1, projections.ElementAt(0)[1]);
            Assert.AreEqual("Spider", projections.ElementAt(1)[0]);
            Assert.AreEqual(2, projections.ElementAt(1)[1]);
        }

        [Test]
        public void SampleDomainQueryWith2SortingOptionsTest()
        {
            IRemoteCache<int, User> userCache = remoteManager.GetCache<int, User>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.User u order by u.name DESC, u.surname ASC";

            QueryResponse result = userCache.Query(qr);

            List<User> list = RemoteQueryUtils.unwrapResults<User>(result);

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("Spider", list.ElementAt(0).Name);
            Assert.AreEqual("Man", list.ElementAt(0).Surname);
            Assert.AreEqual("Spider", list.ElementAt(1).Name);
            Assert.AreEqual("Woman", list.ElementAt(1).Surname);
            Assert.AreEqual("John", list.ElementAt(2).Name);
        }

        [Test]
        public void SortByDateTest()
        {
            IRemoteCache<int, Transaction> txCache = remoteManager.GetCache<int, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t order by t.date ASC";
            QueryResponse result = txCache.Query(qr);
            List<Transaction> listOfTx = RemoteQueryUtils.unwrapResults<Transaction>(result);

            Assert.AreEqual(MakeDate("2012-09-07"), listOfTx.ElementAt(0).Date);
            Assert.AreEqual(MakeDate("2013-02-27"), listOfTx.ElementAt(listOfTx.Count - 1).Date);
        }




        private void PutUsers(IRemoteCache<int, User> remoteCache)
        {
            User user1 = new User();
            user1.Id = 1;
            user1.Name = "John";
            user1.Surname = "Doe";
            user1.Gender = User.Types.Gender.Male;
            user1.Age = 22;
            user1.Notes = "Lorem ipsum dolor sit amet";
            List<Int32> accountIds = new List<Int32>();
            accountIds.Add(1);
            accountIds.Add(2);
            user1.AccountIds.Add(accountIds);
            User.Types.Address address1 = new User.Types.Address();
            address1.Street = "Main Street";
            address1.PostCode = "X1234";
            address1.Number = 156;
            List<User.Types.Address> addresses = new List<User.Types.Address>();
            addresses.Add(address1);
            user1.Addresses.Add(addresses);

            remoteCache.Put(1, user1);

            User user2 = new User();
            user2.Id = 2;
            user2.Name = "Spider";
            user2.Surname = "Man";
            user2.Gender = User.Types.Gender.Male;
            accountIds = new List<Int32>();
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
            addresses = new List<User.Types.Address>();
            addresses.Add(address2);
            addresses.Add(address3);
            user2.Addresses.Add(addresses);

            remoteCache.Put(2, user2);

            User user3 = new User();
            user3.Id = 3;
            user3.Name = "Spider";
            user3.Surname = "Woman";
            user3.Gender = User.Types.Gender.Female;

            remoteCache.Put(3, user3);
            accountIds = new List<Int32>();
            user3.AccountIds.Add(accountIds);
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
            return (ulong) current.Subtract(inception).TotalMilliseconds;
        } 
    }
}
