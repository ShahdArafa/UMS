using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UMS.Core.Entities.Identity;
using UMS.Repository.Services;

namespace UMS.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;

        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager)
        {
            //1.Key
            var Authkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));

            //2.Private Claims
            var Authclaims = new List<Claim>
             {
                 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                //new Claim(ClaimTypes.Role, user.Role) //from ChatGpt
             };

            var UserRoles = await userManager.GetRolesAsync(user);
            foreach (var role in UserRoles) { Authclaims.Add(new Claim(ClaimTypes.Role, role)); }

            //Obj of Token
            var Token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                expires: DateTime.Now.AddMinutes(double.Parse(configuration["JwtSettings:ExpirationMinutes"])),
                claims: Authclaims,
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(Authkey, SecurityAlgorithms.HmacSha256Signature)
                );
            //
            return new JwtSecurityTokenHandler().WriteToken(Token);

        }
    }
}
