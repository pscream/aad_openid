using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

using WebApi.Models.Requests;
using WebApi.Models.Responses;
using WebApi.Security.Entities;
using WebApi.Security.Services;

namespace WebApi.Controllers
{

    [ApiController]
    [Area("api")]
    public class LoginController : ControllerBase
    {

        private readonly ISecurityManager _securityManager;

        public LoginController(ISecurityManager securityManager)
        {
            _securityManager = securityManager;
        }

        [HttpPost("[area]/login")]
        public async Task<ActionResult<JwtToken>> Login([FromBody] Login model)
        {
            var token = await _securityManager.CreateToken(model.UserName, model.Password);
            if (token == null)
                return StatusCode(StatusCodes.Status403Forbidden);

            return new JwtToken(token);
        }

        [HttpGet("[area]/challenge")]
        public ActionResult GetChallenge()
        {
            var properties = new OpenIdConnectChallengeProperties();
            properties.RedirectUri = Url.Action(nameof(AfterOidc));
            return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet("[area]/after-oidc")]
        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
        public ActionResult AfterOidc()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok("Authenticated");
            }

            return Ok("Not Authenticated");
        }

    }

}
