using System;
using System.Collections.Generic;
using System.Linq;
using Infinispan.HotRod.Exceptions;
using Infinispan.HotRod.Config;
using Org.Infinispan.Query.Remote.Client;
using System.IO;
using SampleBankAccount;
using NUnit.Framework;

/**
 * This is a copy of QueryStringTest.java, modified for C#.
 * 
 */
namespace Infinispan.HotRod.Tests.ClusteredIndexingXml
{
    [TestFixture]
    [Category("clustered_indexing_xml")]
    [Category("RemoteQueryTestSuite")]
    class RemoteFullTextQueryTest
    {
        RemoteCacheManager remoteManager;
        const String ERRORS_KEY_SUFFIX = ".errors";
        const String PROTOBUF_METADATA_CACHE_NAME = "___protobuf_metadata";
        const String NAMED_CACHE = "InMemoryNonSharedIndexFullText";

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

            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);
            PutTransactions(transactionCache);
        }

        [Test]
        public void TestExactMatch()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where t.description = 'Birthday present'";

            QueryResponse result = transactionCache.Query(qr);

            List<Transaction> transactions = RemoteQueryUtils.unwrapResults<Transaction>(result);
            Assert.AreEqual(1, transactions.Count);
        }

        [Test]
        public void TestFullTextTerm()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where longDescription:'rent'";

            QueryResponse result = transactionCache.Query(qr);

            List<Transaction> transactions = RemoteQueryUtils.unwrapResults<Transaction>(result);
            Assert.AreEqual(1, transactions.Count);
        }

        [Test]
        [Ignore("Some reason")]
        public void TestFullTextTermRightOperandAnalyzed()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where longDescription:'RENT'";

            QueryResponse result = transactionCache.Query(qr);

            List<Transaction> transactions = RemoteQueryUtils.unwrapResults<Transaction>(result);
            Assert.AreEqual(1, transactions.Count);
        }

        [Test]
        public void TestFullTextTermBoost()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where t.longDescription:('rent'^8 'shoes')";

            QueryResponse result = transactionCache.Query(qr);

            List<Transaction> transactions = RemoteQueryUtils.unwrapResults<Transaction>(result);
            Assert.AreEqual(51, transactions.Count);
        }

        [Test]
        public void TestFullTextPhrase()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where t.longDescription:'expensive shoes'";

            QueryResponse result = transactionCache.Query(qr);

            List<Transaction> transactions = RemoteQueryUtils.unwrapResults<Transaction>(result);
            Assert.AreEqual(50, transactions.Count);
        }

        [Test]
        public void TestFullTextWithAggregation()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "select t.accountId, max(t.amount), max(t.description) from sample_bank_account.Transaction t where t.longDescription : (+'beer' -'food') group by t.accountId";

            QueryResponse result = transactionCache.Query(qr);

            List<Object[]> projections = RemoteQueryUtils.unwrapWithProjection(result);
            Assert.AreEqual(1, projections.Count);
            Assert.AreEqual(2, projections.ElementAt(0)[0]);
            Assert.AreEqual(149.0, (double)projections.ElementAt(0)[1], 0.001d);
            Assert.AreEqual("Expensive shoes 9", projections.ElementAt(0)[2]);
        }

        //sorting does not work: HRCPP-301
        //public void testFullTextTermBoostAndSorting() throws Exception
        //{
        //    QueryFactory qf = getQueryFactory();

        //    Query q = qf.create("from " + getModelFactory().getTransactionTypeName() + " where longDescription:('rent'^8 'shoes') order by amount");

        //    List<Transaction> list = q.list();
        //    assertEquals(51, list.size());
        //}

        [Test]
        public void TestFullTextTermOccur()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where not (t.longDescription : (+'failed') or t.longDescription : 'blocked')";

            QueryResponse result = transactionCache.Query(qr);

            List<Transaction> transactions = RemoteQueryUtils.unwrapResults<Transaction>(result);
            Assert.AreEqual(56, transactions.Count);
        }

        [Test]
        [Ignore("ISPN-7300")]
        public void TestFullTextTermDoesntOccur()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where t.longDescription : (-'really')";

            QueryResponse result = transactionCache.Query(qr);

            List<Transaction> transactions = RemoteQueryUtils.unwrapResults<Transaction>(result);
            Assert.AreEqual(6, transactions.Count);
        }

        [Test]
        public void TestFullTextRange()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where t.longDescription : [* to *]";

            QueryResponse result = transactionCache.Query(qr);

            List<Transaction> transactions = RemoteQueryUtils.unwrapResults<Transaction>(result);
            Assert.AreEqual(54, transactions.Count);
        }

        [Test]
        public void TestFullTextPrefix()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where t.longDescription : 'ren*'";

            QueryResponse result = transactionCache.Query(qr);

            List<Transaction> transactions = RemoteQueryUtils.unwrapResults<Transaction>(result);
            Assert.AreEqual(1, transactions.Count);
        }

        [Test]
        public void TestFullTextWildcard()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where t.longDescription : 're?t'";

            QueryResponse result = transactionCache.Query(qr);

            List<Transaction> transactions = RemoteQueryUtils.unwrapResults<Transaction>(result);
            Assert.AreEqual(1, transactions.Count);
        }

        [Test]
        public void TestFullTextWildcardFuzzyNotAllowed()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where t.longDescription : 're?t'~2";

            Assert.Throws<HotRodClientException>(() => transactionCache.Query(qr));
        }

        [Test]
        public void TestFullTextFuzzy()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where t.longDescription : 'retn'~";

            QueryResponse result = transactionCache.Query(qr);

            List<Transaction> transactions = RemoteQueryUtils.unwrapResults<Transaction>(result);
            Assert.AreEqual(1, transactions.Count);
        }

        [Test]
        public void TestFullTextRegexp()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where t.longDescription : /[R|r]ent/";

            QueryResponse result = transactionCache.Query(qr);

            List<Transaction> transactions = RemoteQueryUtils.unwrapResults<Transaction>(result);
            Assert.AreEqual(1, transactions.Count);
        }

        [Test]
        public void TestFullTextRegexpFuzzyNotAllowed()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where t.longDescription : /[R|r]ent/~2";

            Assert.Throws<HotRodClientException>(() => transactionCache.Query(qr));
        }

        [Test]
        public void TestExactMatchOnAnalyzedFieldNotAllowed()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where t.longDescription = 'Birthday present'";

            Assert.Throws<HotRodClientException>(() => transactionCache.Query(qr));
        }

        public void TestFullTextTermOnNonAnalyzedFieldNotAllowed()
        {
            IRemoteCache<String, Transaction> transactionCache = remoteManager.GetCache<String, Transaction>(NAMED_CACHE);

            QueryRequest qr = new QueryRequest();
            qr.QueryString = "from sample_bank_account.Transaction t where t.description:'rent'";

            Assert.Throws<HotRodClientException>(() => transactionCache.Query(qr));
        }

        private void PutTransactions(IRemoteCache<String, Transaction> remoteCache)
        {
            Transaction transaction0 = new Transaction();
            transaction0.Id = 0;
            transaction0.Description = "Birthday present";
            transaction0.AccountId = 1;
            transaction0.Amount = 1800;
            transaction0.Date = MakeDate("2012-09-07");
            transaction0.IsDebit = false;
            transaction0.IsValid = true;

            remoteCache.Put("transaction_" + transaction0.Id, transaction0);

            Transaction transaction1 = new Transaction();
            transaction1.Id = 1;
            transaction1.Description = "Feb. rent payment";
            transaction1.LongDescription = "Feb. rent payment";
            transaction1.AccountId = 1;
            transaction1.Amount = 1500;
            transaction1.Date = MakeDate("2013-01-05");
            transaction1.IsDebit = true;
            transaction1.IsValid = true;

            remoteCache.Put("transaction_" + transaction1.Id, transaction1);

            Transaction transaction2 = new Transaction();
            transaction2.Id = 2;
            transaction2.Description = "Starbucks";
            transaction2.LongDescription = "Starbucks";
            transaction2.AccountId = 1;
            transaction2.Amount = 23;
            transaction2.Date = MakeDate("2013-01-09");
            transaction2.IsDebit = true;
            transaction2.IsValid = true;

            remoteCache.Put("transaction_" + transaction2.Id, transaction2);

            Transaction transaction3 = new Transaction();
            transaction3.Id = 3;
            transaction3.Description = "Hotel";
            transaction3.AccountId = 2;
            transaction3.Amount = 45;
            transaction3.Date = MakeDate("2013-02-27");
            transaction3.IsDebit = true;
            transaction3.IsValid = true;

            remoteCache.Put("transaction_" + transaction3.Id, transaction3);

            Transaction transaction4 = new Transaction();
            transaction4.Id = 4;
            transaction4.Description = "Last january";
            transaction4.LongDescription = "Last january";
            transaction4.AccountId = 2;
            transaction4.Amount = 95;
            transaction4.Date = MakeDate("2013-01-31");
            transaction4.IsDebit = true;
            transaction4.IsValid = true;

            remoteCache.Put("transaction_" + transaction4.Id, transaction4);

            Transaction transaction5 = new Transaction();
            transaction5.Id = 5;
            transaction5.Description = "-Popcorn";
            transaction5.LongDescription = "-Popcorn";
            transaction5.AccountId = 2;
            transaction5.Amount = 5;
            transaction5.Date = MakeDate("2013-01-01");
            transaction5.IsDebit = true;
            transaction5.IsValid = true;

            remoteCache.Put("transaction_" + transaction5.Id, transaction5);

            for (int i = 0; i < 50; i++)
            {
                Transaction transaction = new Transaction();
                transaction.Id = 50 + i;
                transaction.Description = "Expensive shoes " + i;
                transaction.LongDescription = "Expensive shoes. Just beer, really " + i;
                transaction.AccountId = 2;
                transaction.Amount = 100 + i;
                transaction.Date = MakeDate("2013-08-20");
                transaction.IsDebit = true;
                transaction.IsValid = true;
                remoteCache.Put("transaction_" + transaction.Id, transaction);
            }
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
