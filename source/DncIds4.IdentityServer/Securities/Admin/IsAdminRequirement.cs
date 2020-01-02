using Microsoft.AspNetCore.Authorization;

namespace DncIds4.IdentityServer.Securities.Admin
{
    public class IsAdminRequirement : IAuthorizationRequirement
    {
        public string ApiAdminRole { get; }

        public IsAdminRequirement(string apiAdminRole)
        {
            this.ApiAdminRole = apiAdminRole;
        }
    }
}
