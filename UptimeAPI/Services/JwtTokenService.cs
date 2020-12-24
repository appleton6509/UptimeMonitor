using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UptimeAPI.Settings;

namespace UptimeAPI.Services
{
    public class JwtTokenService
    {
        public string GenerateToken(IdentityUser user, JwtSettings jwtSettings)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(jwtSettings.ExpirationInDays));

            var token = new JwtSecurityToken(jwtSettings.Issuer, jwtSettings.Issuer, claims, null, expires, creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
