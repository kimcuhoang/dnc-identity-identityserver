namespace DncIds4.ApiGatewayOcelot.Config
{
    public class Constants
    {
        public struct IdentityResource
        {
            public const string UserRoles = "user::roles";
            public const string UserScopes = "user::scope";
        }

        public struct ApiRole
        {
            public const string ApiAdmin = "api::admin";
            public const string ApiUser = "api::user";
        }

        public struct Apis
        {
            public const string ResourceApi = "ResourceApi";
            public const string AccountApi = "AccountApi";
        }
    }
}
