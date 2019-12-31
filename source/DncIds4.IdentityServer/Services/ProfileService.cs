using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DncIds4.IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileService(UserManager<IdentityUser> userManager)
        {
            this._userManager = userManager;
        }

        #region Implementation of IProfileService

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject.GetSubjectId();
            var user = await this._userManager.FindByIdAsync(subject);
            var claims = await this._userManager.GetClaimsAsync(user);

            context.IssuedClaims = claims.Select(x => new Claim(x.Type, x.Value)).ToList();
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject.GetSubjectId();
            var user = await this._userManager.FindByIdAsync(subject);
            context.IsActive = user != null;
        }

        #endregion
    }
}
