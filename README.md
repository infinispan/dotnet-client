# Infinispan.Hotrod

[![NuGet](https://img.shields.io/nuget/v/Infinispan.Hotrod)](https://www.nuget.org/packages/Infinispan.Hotrod)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Infinispan.Hotrod)](https://www.nuget.org/packages/Infinispan.Hotrod)
[![CI](https://github.com/infinispan/Infinispan.Hotrod/actions/workflows/test-on-repo.yml/badge.svg)](https://github.com/infinispan/Infinispan.Hotrod/actions/workflows/test-on-repo.yml)
[![License](https://img.shields.io/github/license/infinispan/dotnet-client)](LICENSE)

A .NET client for the [Infinispan](https://infinispan.org) Hot Rod protocol.

## Quick Start

Add the package to your project:

```xml
<PackageReference Include="Infinispan.Hotrod" Version="10.0.0-beta.2" />
```

Connect to an Infinispan cluster and use a cache:

```csharp
var client = InfinispanClient.FromUri("hotrod://admin:password@127.0.0.1:11222");

var cache = client.NewCache(
    new StringMarshaller(), new StringMarshaller(), "default");

await cache.Put("key", "value");
var result = await cache.Get("key");
```

The URI supports multiple hosts (`hotrod://host1,host2,host3`), TLS (`hotrods://...`), and query parameters (`?sasl_mechanism=PLAIN`).

### LINQ Queries

Query indexed caches with standard C# LINQ expressions — the provider translates them to Ickle queries automatically:

```csharp
using Infinispan.Hotrod.Linq;

var cache = client.NewCache<Person>("people")
    .WithEncoding(MediaType.Protobuf)
    .Build();

var results = await cache.AsQueryable()
    .Where(p => p.BornIn == "London" && p.BornYear > 1989)
    .OrderBy(p => p.LastName)
    .Take(10)
    .ToListAsync();
```

String matching, projections, pagination, and terminal operators (`CountAsync`, `FirstAsync`, `SingleAsync`) are all supported. See the [query documentation](documentation/topics/queries.adoc) for details.

A full working example is in the [Infinispan.Hotrod.Samples](Infinispan.Hotrod.Samples) folder. More tutorials are available at [infinispan.org](https://infinispan.org/tutorials/?language=c%23).

## Features

Supports the [Hot Rod 4.1](https://infinispan.org/docs/stable/titles/hotrod_protocol/hotrod_protocol.html) protocol with a pipelining architecture (single connection per server, multiplexed requests):

- **CRUD**: Get, GetWithVersion, GetWithMetadata, Put, PutIfAbsent, Replace, ReplaceWithVersion, Remove, RemoveWithVersion
- **Bulk**: PutAll, GetAll, KeySet, Clear, Size
- **Iteration**: Server-side iteration with `EntrySet`, `Values`, `RetrieveEntries`, `RetrieveEntriesWithMetadata` (`IAsyncEnumerable`)
- **Transactions**: Client-side transaction buffering with optimistic locking, one-phase and two-phase commit
- **Near Caching**: Client-side LRU cache with server-driven invalidation via event listeners
- **Query**: Ickle query language and LINQ-to-Ickle provider
- **Continuous Queries**: Real-time notifications when cache entries match or stop matching a query
- **Stats**: Server-side cache statistics
- **Events**: Client listeners with server-push event dispatch
- **Counters**: Distributed strong and weak counters with get, set, add, compare-and-swap, reset
- **Multimaps**: Associate multiple values with a single key (put, get, remove key/entry, contains, size)
- **Administration**: Cache lifecycle (create, get-or-create, remove, list), templates, index management, schema registration, server task execution
- **Security**: TLS (with optional server certificate verification), SASL authentication (PLAIN, SCRAM-SHA-256/384/512, GSSAPI, EXTERNAL)
- **Topology**: Client intelligence with automatic cluster topology updates and failover
- **ASP.NET Integration**: `IDistributedCache` and `HybridCache` implementations

## Requirements

- .NET 10 or later
- Docker (for running tests — [Testcontainers](https://dotnet.testcontainers.org/) manages the Infinispan server automatically)

## Building and Testing

```sh
dotnet build
dotnet test
```

Tests use Testcontainers to automatically pull and run an Infinispan server image — no manual server setup required.

## Documentation

API documentation is generated with [DocFX](https://dotnet.github.io/docfx/). To build and serve locally:

```sh
dotnet tool install -g docfx
docfx docfx.json --serve
```

## Contributing

Contributions are welcome. Please open an [issue](https://github.com/infinispan/Infinispan.Hotrod/issues) to discuss features or report bugs, or submit a pull request.

## License

[Apache License 2.0](LICENSE)
