using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DncIds4.ProtectedApi.Securities.Admin
{
    public class IsAdminInScopeAuthorizationHandler : AuthorizationHandler<IsAdminRequirement>
    {
        #region Overrides of AuthorizationHandler<IsAdminRequirement>

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminRequirement requirement)
        {
            if (context.User.HasClaim(claim => claim.Type == "scope" && claim.Value == requirement.AdminScope))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }

        #endregion
    }
}
