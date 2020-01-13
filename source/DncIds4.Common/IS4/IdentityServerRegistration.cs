using DncIds4.Common.IS4.Extensions;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DncIds4.Common.IS4
{
    public static class IdentityServerRegistration
    {
        public static IServiceCollection AddIdentityServer4(this IServiceCollection services, IConfiguration configuration)
        {
            var is4Config = configuration.GetIdentityServerConfig();

            services.AddSingleton(is4Config);

            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(opts =>
                {
                    opts.Authority = is4Config.IdentityServerUrl;
                    opts.RequireHttpsMetadata = false;
                    opts.ApiName = is4Config.ApiName;
                    opts.ApiSecret = is4Config.ClientSecret;
                });

            return services;
        }
    }
}
