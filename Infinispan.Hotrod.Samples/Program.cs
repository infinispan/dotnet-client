using System;
using Infinispan.Hotrod;

var client = InfinispanClient.FromUri(
    "hotrod://admin:admin@127.0.0.1:11222?client_intelligence=hash-distribution-aware");
client.ForceReturnValue = false;

var cache = client.NewCache<string>("distributed")
    .WithEncoding(MediaType.PlainText)
    .Build();
cache.ForceReturnValue = true;

string result = await cache.Put("key1", "value1");
Console.WriteLine("Result is: " + result);

string getResult = await cache.Get("key1");
Console.WriteLine("Get Result is: " + getResult);
