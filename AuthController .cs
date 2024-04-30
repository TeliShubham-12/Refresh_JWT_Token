// AuthController.cs
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Entities;
using Api.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace JwtAuthExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly string _jwtSecret;
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
            _jwtSecret = "Authentication_Using_JWT_Token_And_Jwt_RefreshToken_JWT_Authentication_And_Authorization"; // You should use a secure key in production
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var user = await _authRepository.GetUserAsync(model.Username, model.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = GenerateJwtToken(user.Username);
            var refreshToken = GenerateRefreshToken();
            await _authRepository.AddRefreshTokenAsync(user.Username, refreshToken);

            return Ok(new { token, refreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(string username, string refreshToken)
        {
            var user = await _authRepository.GetUserByRefreshTokenAsync(username, refreshToken);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            var token = GenerateJwtToken(user.Username);
            return Ok(new { token });
        }

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(15), // Access token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var refreshToken = Guid.NewGuid().ToString();
            return refreshToken;
        }
        
           [HttpGet("check-auth")]
           public IActionResult CheckAuthentication()
        {
            
            var username = User.Identity.Name;
            return Ok(new { message = $"Authenticated as {username}" });
        }

    }
}
