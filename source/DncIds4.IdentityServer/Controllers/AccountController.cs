using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DncIds4.IdentityServer.Services;
using DncIds4.IdentityServer.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DncIds4.IdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            this._accountService = accountService;
        }

        [HttpGet("ping")]
        [AllowAnonymous]
        public IActionResult Ping() => Ok("Pong!!!!!!!!!!!!!!!!!!");

        [HttpGet("claims")]
        public IActionResult GetClaims()
        {
            var temp = from c in User.Claims select new { c.Type, c.Value };
            return Ok(temp);
        }

        [HttpPost("register")]
        [Authorize(Policy = "For_Admin")]
        public async Task<IActionResult> Register(AccountRegistration model)
        {
            await this._accountService.AccountRegister(model);
            return Ok($"Account created at {DateTime.Now}");
        }
    }
}
