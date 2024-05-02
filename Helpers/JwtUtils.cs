using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Entities;

namespace WebApi.Helpers;

public interface IJwtUtils
{
    string GenerateJwtToken(IEnumerable<Claim> claims);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    string GenerateRefreshToken();
}

public class JwtUtils : IJwtUtils
{
    private readonly AppSettings _appSettings;

    public JwtUtils(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = "http://localhost:5235",
            Audience = "http://localhost:5235",
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddMinutes(_appSettings.AccessTokenTTL),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "http://localhost:5235",
            ValidAudience = "http://localhost:5235",
            ValidateLifetime = false, //here we are saying that we don't care about the token's expiration date
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(
            token,
            tokenValidationParameters,
            out securityToken
        );
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (
            jwtSecurityToken == null
            || !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase
            )
        )
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }
}
