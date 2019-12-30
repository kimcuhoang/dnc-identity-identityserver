using System.Collections.Generic;
using DncIds4.IdentityServer.Config;

namespace DncIds4.IdentityServer.Services.Models
{
    public class AccountRegistration
    {
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
