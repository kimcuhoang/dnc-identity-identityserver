using System;
using System.Collections.Generic;

namespace DncIds4.IdentityServer.Services.Exceptions
{
    public class AccountRegistrationException : BaseException
    {
        public List<string> Errors { get; set; } = new List<string>();

        public AccountRegistrationException(string message) : base(message)
        {
        }

        public AccountRegistrationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
