using AppStore.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AppStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Authentication(User user)
        {
            if (IsValidUser(user))
            {
                var token = GenerateToken();
                return Ok(new { token });
            }

            return NotFound();
        }

        private string GenerateToken()
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var header = new JwtHeader(signingCredentials);

            var claims = new[] {
                new Claim(ClaimTypes.Name, "Estiben Licona"),
                new Claim(ClaimTypes.Email, "estibenlicona@gmail.com"),
                new Claim(ClaimTypes.Role, "Administrator")
            };

            var payload = new JwtPayload(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claims,
                DateTime.Now,
                DateTime.UtcNow.AddMinutes(2)
            );

            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool IsValidUser(User user)
        {
            if (user.Username == "estibenlicona" && user.Password == "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9")
            {
                return true;
            }
            return false;
        }
    }
}
