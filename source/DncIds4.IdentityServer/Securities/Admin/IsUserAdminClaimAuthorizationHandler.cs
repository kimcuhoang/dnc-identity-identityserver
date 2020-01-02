using System.Threading.Tasks;
using DncIds4.IdentityServer.Config;
using Microsoft.AspNetCore.Authorization;

namespace DncIds4.IdentityServer.Securities.Admin
{
    public class IsUserAdminClaimAuthorizationHandler : AuthorizationHandler<IsAdminRequirement>
    {
        #region Overrides of AuthorizationHandler<IsAdminRequirement>

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminRequirement requirement)
        {
            if (context.User.HasClaim(claim => claim.Type == Constants.IdentityResource.UserRoles && claim.Value == requirement.ApiAdminRole))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }

        #endregion
    }
}
