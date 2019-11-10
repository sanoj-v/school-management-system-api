using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DotNetCoreAPI.Models;
using DotNetCoreAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DotNetCoreAPI.Controllers
{
    [Route("[controller]/[action]")]
    public class TokenController : Controller
    {
        private IConfiguration _config;
        public TokenController(IConfiguration config)
        {
            _config = config;
        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateToken([FromBody]LoginModel login)
        {
            IActionResult response = Unauthorized();
            var user = UserRepository.Authenticate(login);

            if (user != null)
            {
                var tokenString = BuildToken(user);
                var requestAt = DateTime.Now;
                response = Ok(new
                {
                    requestAt = requestAt,
                    expiresIn = TimeSpan.FromMinutes(30).TotalSeconds,
                    tokeyType = "Bearer",
                    accessToken = tokenString
                });
            }

            return response;
        }
        private string BuildToken(UserModel user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] { new Claim(ClaimTypes.Name, user.Name.ToString()), new Claim("userId", user.Id.ToString()) };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds,
              claims: claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal UserId
        {
            get
            {
                return HttpContext?.User;
            }
        }
    }
}