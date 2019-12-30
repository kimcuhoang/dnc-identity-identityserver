using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DncIds4.ProtectedApi.Securities.Admin
{
    public class IsAdminClaimAuthorizationHandler : AuthorizationHandler<IsAdminRequirement>
    {
        #region Overrides of AuthorizationHandler<IsAdminRequirement>

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminRequirement requirement)
        {
            if (context.User.HasClaim(claim => claim.Type == "role" && claim.Value == requirement.ApiAdminRole))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }

        #endregion
    }
}
