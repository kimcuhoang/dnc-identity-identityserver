﻿using System;
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
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiResource> ApiResources => 
            new ApiResource[]
            {
                new ApiResource("ApiResource", "The resource Api is protected by IdentityServer4")
                {
                    Scopes =
                    {
                        new Scope("ApiResource.Read", "Read Only access to ApiResource")
                    },
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                }, 
            };

        public static IEnumerable<Client> Clients => 
            new Client[]
            {
                new Client
                {
                    ClientId = "ApiResource",
                    AllowAccessTokensViaBrowser = true,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {
                        "ApiResource",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    AllowedCorsOrigins = {"http://localhost:5002"}
                }
            };

        public static List<TestUser> TestUsers =>
            new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "sub1",
                    Username = "test.user",
                    Password = "test123",
                    //Claims = new List<Claim>
                    //{
                    //    new Claim("ApiResource", "ApiResource")
                    //}
                }, 
            };
    }
}
