using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                    AllowedGrantTypes = GrantTypes.Implicit
                }
            };

        public static List<TestUser> TestUsers =>
            new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "sub1",
                    Username = "test.user",
                    Password = "test123"
                }, 
            };
    }
}
