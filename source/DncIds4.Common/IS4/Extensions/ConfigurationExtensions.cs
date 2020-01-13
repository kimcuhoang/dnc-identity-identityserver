using Microsoft.Extensions.Configuration;

namespace DncIds4.Common.IS4.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IdentityServerConfig GetIdentityServerConfig(this IConfiguration configuration)
        {
            return configuration.GetSection("IdentityServerConfig").Get<IdentityServerConfig>();
        }
    }
}
