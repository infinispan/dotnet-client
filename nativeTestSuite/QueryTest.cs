using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Infinispan.HotRod;
using Infinispan.HotRod.Config;
using Org.Infinispan.Query.Remote.Client;
using System.IO;
using Google.Protobuf;
using Org.Infinispan.Protostream;
using SampleBankAccount;
namespace nativeTestSuite
{
    class QueryNativeTest
    {
        const String ERRORS_KEY_SUFFIX = ".errors";
        const String PROTOBUF_METADATA_CACHE_NAME = "___protobuf_metadata";

        static void Main(string[] args)
        {
            ConfigurationBuilder conf = new ConfigurationBuilder();
            conf.AddServer().Host("127.0.0.1").Port(11222).ConnectionTimeout(90000).SocketTimeout(900);
            conf.Marshaller(new BasicTypesProtoStreamMarshaller());
            RemoteCacheManager remote = new RemoteCacheManager(conf.Build(), true);
            IRemoteCache<String,String> metadataCache = remote.GetCache<String, String>(PROTOBUF_METADATA_CACHE_NAME);
            IRemoteCache<int, User> testCache = remote.GetCache<int, User>("namedCache");
            String path = (args.Length > 0) ? args[0] : "";
            metadataCache.Put("sample_bank_account/bank.proto", File.ReadAllText(path+"/query_proto/bank.proto"));
            if (metadataCache.ContainsKey(ERRORS_KEY_SUFFIX))
            {
                System.Console.WriteLine("fail: error in registering .proto model");
                Environment.Exit(-1);
            }

            User user1 = new User();
            user1.Id = 4;
             user1.Name= "Jerry";
            user1.Surname="Mouse";

             User ret = testCache.Put(4, user1);


            QueryRequest qr = new QueryRequest();
            qr.JpqlString="from sample_bank_account.User";


            QueryResponse result=testCache.Query(qr);
            List<User> l = new List<User>();
            unwrapResults(result, l);

            System.Console.WriteLine("result string is: "+l);


        }

        private static bool unwrapResults<T>(QueryResponse resp, List<T> res) where T : IMessage<T>
        {
            if (resp.ProjectionSize > 0)
            {  // Query has select
                return false;
            }
            for (int i = 0; i < resp.NumResults; i++)
            {
                WrappedMessage wm = resp.Results.ElementAt(i);

                if (wm.WrappedBytes != null)
                {
                    WrappedMessage wmr = WrappedMessage.Parser.ParseFrom(wm.WrappedBytes);
                    if (wmr.WrappedMessageBytes != null)
                    {
                        System.Reflection.PropertyInfo pi = typeof(T).GetProperty("Parser");

                        MessageParser<T> p = (MessageParser<T>)pi.GetValue(null);
                        T u = p.ParseFrom(wmr.WrappedMessageBytes);
                        res.Add(u);
                    }
                }
            }
            return true;
        }

    }
}
