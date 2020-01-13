using Consul;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DncIds4.Common.Consul
{
    public class ConsulHostedService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly AgentServiceRegistration _agentServiceRegistration;

        public ConsulHostedService(IConsulClient consulClient, ConsulConfig consulConfig,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            this._consulClient = consulClient;
            this._hostApplicationLifetime = hostApplicationLifetime;

            this._agentServiceRegistration = new AgentServiceRegistration
            {
                ID = consulConfig.ServiceName,
                Name = consulConfig.ServiceName,
                Address = consulConfig.HostName,
                Port = consulConfig.Port,
                Tags = new[] { "Consul", consulConfig.ServiceName },
                Check = new AgentServiceCheck()
                {
                    HTTP = $"http://{consulConfig.HostName}:{consulConfig.Port}/{consulConfig.PingEndpoint}",
                    Timeout = TimeSpan.FromSeconds(3),
                    Interval = TimeSpan.FromSeconds(consulConfig.PingInterval)
                }
            };
        }

        #region Implementation of IHostedService

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._hostApplicationLifetime.ApplicationStarted.Register(async () =>
            {
                await this._consulClient.Agent.ServiceDeregister(this._agentServiceRegistration.ID, cancellationToken);
                await this._consulClient.Agent.ServiceRegister(this._agentServiceRegistration, cancellationToken);
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._hostApplicationLifetime.ApplicationStopping.Register(async () =>
            {
                await this._consulClient.Agent.ServiceDeregister(this._agentServiceRegistration.ID, cancellationToken);
            });

            return Task.CompletedTask;
        }

        #endregion
    }
}
