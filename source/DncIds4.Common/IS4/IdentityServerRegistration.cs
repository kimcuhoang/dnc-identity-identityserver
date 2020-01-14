using DncIds4.Common.IS4.Extensions;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DncIds4.Common.IS4
{
    public static class IdentityServerRegistration
    {
        public static IServiceCollection AddIdentityServer4(this IServiceCollection services, IdentityServerConfig identityServerConfig)
        {
            services
                .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(opts =>
                {
                    opts.Authority = identityServerConfig.IdentityServerUrl;
                    opts.RequireHttpsMetadata = false;
                    opts.ApiName = identityServerConfig.ApiName;
                    opts.ApiSecret = identityServerConfig.ClientSecret;
                });

            return services;
        }
    }
}
