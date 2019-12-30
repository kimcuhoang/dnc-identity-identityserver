using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DncIds4.ProtectedApi.Securities.Admin
{
    public class IsAdminRequirement : IAuthorizationRequirement
    {
        public string AdminScope { get; }

        public string ApiAdminRole { get; }

        public IsAdminRequirement(string adminScope, string apiAdminRole)
        {
            this.AdminScope = adminScope;
            this.ApiAdminRole = apiAdminRole;
        }
    }
}
