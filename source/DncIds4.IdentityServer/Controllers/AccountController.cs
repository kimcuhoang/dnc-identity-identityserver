using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DncIds4.IdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok("Pong!!!!!!!!!!!!!!!!!!");

        [HttpGet("claims")]
        public IActionResult GetClaims()
        {
            var temp = from c in User.Claims select new { c.Type, c.Value };
            return Ok(temp);
        }

        [HttpPost("register")]
        [Authorize(Policy = "For_Admin")]
        public IActionResult Register()
        {
            return Ok("Register Successfully");
        }
    }
}
