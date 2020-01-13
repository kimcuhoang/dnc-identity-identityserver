using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DncIds4.Common.Consul
{
    public static class ConsulRegistration
    {
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            var consulConfig = configuration.GetSection("consul").Get<ConsulConfig>();
            services.AddSingleton(consulConfig);

            services.AddSingleton<IConsulClient>(new ConsulClient(cfg =>
            {
                cfg.Address = consulConfig.ConsulServiceDiscoveryUrl;
            }));

            services.AddHostedService<ConsulHostedService>();

            return services;
        }
    }
}
