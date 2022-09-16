using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using WebApi.Security.Entities;

namespace WebApi.Security.Services
{

    public class SecurityManager : ISecurityManager
    {

        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public SecurityManager(IConfiguration configuration, UserManager<User> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> CreateToken(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, password)))
                return null;

            return BuildToken(user.Id);
        }

        public async Task<string> CreateExternalUserToken(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null || !user.IsExternal)
                return null;

            return BuildToken(user.Id);
        }

        private string BuildToken(Guid id)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>() { new Claim("UserID", id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApplicationSettings:JwtKey"])),
                                                                SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }

        public async Task CreateOrUpdateExternalUser(string username, Guid externalId)
        {
            var user = await _userManager.FindByNameAsync(username);
            if(user == null)
            {
                var newUser = new User() 
                {
                    Id = Guid.NewGuid(),
                    UserName = username,
                    NormalizedUserName = username.ToUpper(),
                    Email = username,
                    NormalizedEmail = username.ToUpper(),
                    IsExternal = true,
                    ExternalId = externalId,
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(newUser);
            }
        }

    }

}
