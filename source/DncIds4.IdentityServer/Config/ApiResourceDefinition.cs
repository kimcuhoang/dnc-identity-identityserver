using System.Collections.Generic;
using IdentityServer4;

namespace DncIds4.IdentityServer.Config
{
    public class ApiResourceDefinition
    {
        public enum Apis
        {
            IdentityServerApi,
            ResourceApi
        }

        public static Dictionary<Apis, string> ApiResources => new Dictionary<Apis, string>
        {
            { Apis.IdentityServerApi, $"{IdentityServerConstants.LocalApi.ScopeName}"},
            { Apis.ResourceApi, $"{nameof(Apis.ResourceApi)}" }
        };
    }
}
