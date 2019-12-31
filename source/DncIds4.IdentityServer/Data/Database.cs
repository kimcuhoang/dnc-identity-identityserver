using DncIds4.IdentityServer.Config;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
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
                //List of associated user claim types that should be included in the identity token.
                new IdentityResource(name:ApiRoleDefinition.RoleClaimText, new []{ ApiRoleDefinition.RoleClaimText })
                
            };

        public static IEnumerable<ApiResource> ApiResources => 
            new ApiResource[]
            {
                new ApiResource(ApiResourceDefinition.ApiResources[ApiResourceDefinition.Apis.ResourceApi], "The resource Api is protected by IdentityServer4")
                {
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    //List of associated user claim types that should be included in the access token. The claims specified here will be added to the list of claims specified for the API.
                    UserClaims = { ApiRoleDefinition.RoleClaimText }
                },
                new ApiResource(ApiResourceDefinition.ApiResources[ApiResourceDefinition.Apis.AccountApi], "The API for account management")
                {
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    UserClaims = { ApiRoleDefinition.RoleClaimText }
                },
            };

        public static IEnumerable<Client> Clients => 
            new Client[]
            {
                new Client
                {
                    ClientId = "ResourceApi_Swagger",
                    AllowAccessTokensViaBrowser = true,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    //By default a client has no access to any resources - specify the allowed resources by adding the corresponding scopes names
                    AllowedScopes = {
                        ApiResourceDefinition.ApiResources[ApiResourceDefinition.Apis.ResourceApi]
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
                        ApiResourceDefinition.ApiResources[ApiResourceDefinition.Apis.AccountApi]
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
