using DncIds4.IdentityServer.Config;
using DncIds4.IdentityServer.Services.Exceptions;
using DncIds4.IdentityServer.Services.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DncIds4.IdentityServer.Services
{
    public class AccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        public async Task AccountRegister(AccountRegistration model)
        {
            var identityUser = new IdentityUser
            {
                Email = model.EmailAddress,
                UserName = model.Username
            };

            var user = await this._userManager.FindByEmailAsync(model.EmailAddress);
            user = await this._userManager.FindByNameAsync(model.Username);

            if (user != null)
            {
                throw new Exception($"The {nameof(model.Username)} or {nameof(model.EmailAddress)} has been used.");
            }

            var accountCreationResult = await this._userManager.CreateAsync(identityUser, model.Password);

            if (!accountCreationResult.Succeeded)
            {
                var error = new AccountRegistrationException("Could not register account")
                {
                    Errors = accountCreationResult.Errors.Select(x => x.Description).ToList()
                };

                throw error;
            }

            // Add Claims for user
            var claims = new Claim[]
            {
                new Claim(ApiRoleDefinition.RoleClaimText,
                    model.IsAdmin
                        ? ApiRoleDefinition.ApiRoles[ApiRoleDefinition.Roles.Admin]
                        : ApiRoleDefinition.ApiRoles[ApiRoleDefinition.Roles.User]),
                new Claim(ClaimTypes.Email, identityUser.Email),
                new Claim(ClaimTypes.Name, identityUser.UserName),
            };
            var addClaimResult = await this._userManager.AddClaimsAsync(identityUser, claims);
            if (!addClaimResult.Succeeded)
            {
                var exception = new AccountRegistrationException("Could not add claims to user")
                {
                    Errors = addClaimResult.Errors.Select(x => x.Description).ToList()
                };

                throw exception;
            }
        }
    }
}
