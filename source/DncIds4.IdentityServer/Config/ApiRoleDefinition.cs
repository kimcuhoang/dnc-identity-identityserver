using System.Collections.Generic;

namespace DncIds4.IdentityServer.Config
{
    public class ApiRoleDefinition
    {
        public enum Roles
        {
            Admin,
            User
        }

        public static Dictionary<Roles, string> ApiRoles => new Dictionary<Roles, string>
        {
            { Roles.Admin, "api::admin" },
            { Roles.User, "api::user" }
        };
    }
}
