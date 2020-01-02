namespace DncIds4.ProtectedApi.Config
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
    }
}
