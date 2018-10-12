using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TestTokenow.Data;
using TestTokenow.Models;

namespace TestTokenow.Controllers
{

    public class AuthController : Controller
    {

        private ILogger<AuthController> _logger;
        private SignInManager<User> _signInMgr;
        private UserManager<User> _userMgr;
        private IPasswordHasher<User> _hasher;
        private IConfiguration _config;

        public AuthController(
          SignInManager<User> signInMgr,
          UserManager<User> userMgr,
          IPasswordHasher<User> hasher,
          ILogger<AuthController> logger,
          IConfiguration config)
        {
            _signInMgr = signInMgr;
            _logger = logger;
            _userMgr = userMgr;
            _hasher = hasher;
            _config = config;
        }

        [HttpPost("api/auth/token")]
        public async Task<IActionResult> CreateToken([FromBody] CredentialModel model)
        {
            try
            {
                var user = await _userMgr.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    if (_hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success)
                    {
                        var claims = new List<Claim> {
                            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                           };

                        var userRoles = await _userMgr.GetRolesAsync(user);

                        foreach (var userRole in userRoles)
                        {
                            claims.Add(new Claim("role", userRole));
                            //var role = await _roleManager.FindByNameAsync(userRole);
                            //if (role != null)
                            //{
                            //    var roleClaims = await _roleManager.GetClaimsAsync(role);
                            //    foreach (Claim roleClaim in roleClaims)
                            //    {
                            //        claims.Add(roleClaim);
                            //    }
                            //}
                        }


                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                          issuer: _config["Jwt:Issuer"],
                          audience: _config["Jwt:Audience"],
                          claims: claims,
                          expires: DateTime.UtcNow.AddMinutes(15),
                          signingCredentials: creds
                          );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            //expiration = token.ValidTo
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown while creating JWT: {ex}");
            }

            return BadRequest("Failed to generate token");
        }
    }
}