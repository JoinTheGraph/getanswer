using System;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GetAnswer.DbReader.StartupConfiguration
{
    public static class Setup
    {
        public static void RegisterServices(IServiceCollection services, IConfigurationSection databaseConnectionConfigurationSection)
        {
            DatabaseConnectionConfiguration conf = databaseConnectionConfigurationSection.Get<DatabaseConnectionConfiguration>();

            services.AddSingleton(serviceProvider => CreateGremlinClient(conf.Hostname, conf.Port));

            services.AddSingleton(
                (serviceProvider) =>
                {
                    GremlinClient gremlinClient = serviceProvider.GetService<GremlinClient>();
                    return CreateGraphTraversalSource(gremlinClient, conf.TraversalSource);
                }
            );

            services.AddSingleton<User.AuthenticationReader>();
        }

        private static GremlinClient CreateGremlinClient(string hostname, int port)
        {
            var gremlinServer = new GremlinServer(
                hostname: hostname,
                port: port,
                enableSsl: false,
                username: null,
                password: null
            );

            var connectionPoolSettings = new ConnectionPoolSettings
            {
                MaxInProcessPerConnection = 32,
                PoolSize = 4,
                ReconnectionAttempts = 4,
                ReconnectionBaseDelay = TimeSpan.FromSeconds(1)
            };

            return new GremlinClient(
                gremlinServer: gremlinServer,
                connectionPoolSettings: connectionPoolSettings
            );
        }

        private static GraphTraversalSource CreateGraphTraversalSource(GremlinClient gremlinClient, string traversalSource)
        {
            var driverRemoteConnection = new DriverRemoteConnection(gremlinClient, traversalSource);
            return AnonymousTraversalSource.Traversal().WithRemote(driverRemoteConnection);
        }
    }
}