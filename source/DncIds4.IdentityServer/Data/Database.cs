using DncIds4.IdentityServer.Config;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace DncIds4.IdentityServer.Data
{
    public static class Database
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("role", ApiRoleDefinition.RoleTexts)
            };

        public static IEnumerable<ApiResource> ApiResources => 
            new ApiResource[]
            {
                new ApiResource(ApiResourceDefinition.ApiResources[ApiResourceDefinition.Apis.ResourceApi], "The resource Api is protected by IdentityServer4")
                {
                    Scopes =
                    {
                        new Scope("protected_api", "Read Only access to ApiResource")
                    },
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    UserClaims = ApiRoleDefinition.RoleTexts
                },
                new ApiResource(ApiResourceDefinition.ApiResources[ApiResourceDefinition.Apis.AccountApi], "The API for account management")
                {
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    UserClaims = ApiRoleDefinition.RoleTexts
                },
            };

        public static IEnumerable<Client> Clients => 
            new Client[]
            {
                new Client
                {
                    ClientId = "ApiResource_Swagger",
                    AllowAccessTokensViaBrowser = true,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {
                        ApiResourceDefinition.ApiResources[ApiResourceDefinition.Apis.ResourceApi],
                        ApiRoleDefinition.RoleClaimText,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    AllowedCorsOrigins = {"http://localhost:5002"}
                },
                new Client
                {
                    ClientId = "AccountApi_Swagger",
                    AllowAccessTokensViaBrowser = true,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {
                        ApiResourceDefinition.ApiResources[ApiResourceDefinition.Apis.AccountApi],
                        ApiRoleDefinition.RoleClaimText,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    Claims =
                    {
                        new Claim(ApiRoleDefinition.RoleClaimText, ApiRoleDefinition.ApiRoles[ApiRoleDefinition.Roles.Admin])
                    }
                }
            };

        public static List<TestUser> TestUsers =>
            new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "sub1",
                    Username = "admin",
                    Password = "admin",
                    Claims = new List<Claim>
                    {
                        new Claim("role", "api::admin")
                    }
                },
                new TestUser
                {
                    SubjectId = "sub2",
                    Username = "user",
                    Password = "user123",
                    Claims = new List<Claim>
                    {
                        new Claim("role", "api::user")
                    }
                },
            };
    }
}
