# Infinispan.Hotrod

A .NET client for the Infinispan Hot Rod protocol.

## Quick Start

1. Add the package to your project:

   ```xml
   <PackageReference Include="Infinispan.Hotrod" Version="10.0.0-beta.1" />
   ```

2. Create a cluster connection:

   ```csharp
   var infinispan = new InfinispanClient();
   infinispan.AddHost("127.0.0.1", 11222);
   infinispan.Version = 0x1f;
   infinispan.ForceReturnValue = false;
   infinispan.ClientIntelligence = 0x01;
   ```

3. Create a cache:

   ```csharp
   var cache = infinispan.NewCache(
       new StringMarshaller(), new StringMarshaller(), "default");
   ```

4. Use the cache:

   ```csharp
   await cache.Put("key", "value");
   var result = await cache.Get("key");
   ```

## Cluster Configuration

### Connection Properties

- **TLS**: Enable/disable via `InfinispanClient.UseTLS`. Server certificate verification via `InfinispanClient.CACert`.
- **Authentication**: Set `InfinispanClient.User`, `InfinispanClient.Password`, and `InfinispanClient.AuthMech` (supports PLAIN, SCRAM-SHA-256).

### Cluster Topology

- Add a node to the default cluster: `InfinispanClient.AddHost(host, port)`
- Add a node to a named cluster (for failover): `InfinispanClient.AddHost(cluster, host, port)`
