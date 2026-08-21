using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace Infinispan.Hotrod.Tests.Util
{
    public class InfinispanContainer : IAsyncDisposable
    {
        private const int InfinispanPort = 11222;
        private const string DefaultImage = "quay.io/infinispan/server:16.2";
        private static readonly string ConfDir = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "resources", "conf"));

        private readonly IContainer _container;
        private readonly int? _fixedHostPort;

        public string Host => _container.Hostname;
        public int Port => _fixedHostPort ?? _container.GetMappedPublicPort(InfinispanPort);

        public InfinispanContainer(
            string configFile,
            string user = null,
            string password = null,
            INetwork network = null,
            string[] networkAliases = null,
            IDictionary<string, string> envVars = null,
            int? hostPort = null,
            IDictionary<string, string> resourceMappings = null)
        {
            _fixedHostPort = hostPort;
            var configPath = Path.Combine(ConfDir, configFile);
            var configContent = File.ReadAllBytes(configPath);

            var builder = new ContainerBuilder(DefaultImage)
                .WithExposedPort(InfinispanPort)
                .WithResourceMapping(configContent, "/opt/infinispan/server/conf/infinispan.xml")
                .WithWaitStrategy(
                    Wait.ForUnixContainer()
                        .UntilMessageIsLogged("Infinispan Server .* started in"));

            if (hostPort.HasValue)
                builder = builder.WithPortBinding(hostPort.Value, InfinispanPort);
            else
                builder = builder.WithPortBinding(InfinispanPort, true);

            if (user != null && password != null)
            {
                builder = builder
                    .WithEnvironment("USER", user)
                    .WithEnvironment("PASS", password);
            }

            if (network != null)
            {
                builder = builder.WithNetwork(network);
                if (networkAliases != null)
                    builder = builder.WithNetworkAliases(networkAliases);
            }

            if (envVars != null)
            {
                foreach (var kv in envVars)
                    builder = builder.WithEnvironment(kv.Key, kv.Value);
            }

            if (resourceMappings != null)
            {
                foreach (var kv in resourceMappings)
                {
                    var content = File.ReadAllBytes(kv.Key);
                    builder = builder.WithResourceMapping(content, kv.Value);
                }
            }

            _container = builder.Build();
        }

        public Task StartAsync() => _container.StartAsync();

        public Task StopAsync() => _container.StopAsync();

        public async Task CreateUserAsync(string username, string password, string groups)
        {
            var result = await _container.ExecAsync(new[]
            {
                "/opt/infinispan/bin/cli.sh", "user", "create",
                username, "-p", password, "-g", groups
            });
            if (result.ExitCode != 0)
                throw new Exception($"Failed to create user '{username}': {result.Stderr}");
        }

        public async Task EnsureStartedAsync(int timeoutMs = 60000)
        {
            await _container.StartAsync();
            await WaitForReadyAsync(timeoutMs);
        }

        public async Task WaitForReadyAsync(int timeoutMs = 60000)
        {
            var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    using var client = new TcpClient();
                    await client.ConnectAsync(Host, Port);
                    return;
                }
                catch
                {
                    await Task.Delay(500);
                }
            }
            throw new TimeoutException($"Infinispan server at {Host}:{Port} did not become ready within {timeoutMs}ms");
        }

        public async ValueTask DisposeAsync()
        {
            await _container.DisposeAsync();
        }
    }
}
