using System.Collections.Generic;
using System.Linq;

namespace DncIds4.IdentityServer.Config
{
    public class ApiRoleDefinition
    {
        public static readonly string RoleClaimText = "role";

        public enum Roles
        {
            Admin,
            User,
            Role
        }

        public static Dictionary<Roles, string> ApiRoles => new Dictionary<Roles, string>
        {
            { Roles.Admin, "api::admin" },
            { Roles.User, "api::user" },
            { Roles.Role, RoleClaimText }
        };

        public static string[] RoleTexts => ApiRoles.Select(x => x.Value).ToArray();
    }
}
