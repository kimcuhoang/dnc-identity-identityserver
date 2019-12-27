using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace DncIds4.IdentityServer.Data
{
    public static class Database
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("role", new []
                {
                    "admin",
                    "user",
                    "api::admin",
                    "api::user"
                }), 
                new IdentityResource("account_management", new []
                {
                    "admin",
                    "manager",
                    "user",
                    "account::create",
                    "account::view"
                }), 
            };

        public static IEnumerable<ApiResource> ApiResources => 
            new ApiResource[]
            {
                new ApiResource("ApiResource", "The resource Api is protected by IdentityServer4")
                {
                    Scopes =
                    {
                        new Scope("protected_api", "Read Only access to ApiResource")
                    },
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    UserClaims = {
                        "role",
                        "admin",
                        "user",
                        "api::admin",
                        "api::user"
                    }
                },
                new ApiResource("AccountApi", "The API for account management")
                {
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    UserClaims = {
                        "role",
                        "admin",
                        "user",
                        "manager",
                        "account::create",
                        "account::view"
                    }
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
                        "ApiResource",
                        "role",
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
                        "AccountApi",
                        "role",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
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
                    Password = "user",
                    Claims = new List<Claim>
                    {
                        new Claim("role", "api::user")
                    }

                },
            };
    }
}
