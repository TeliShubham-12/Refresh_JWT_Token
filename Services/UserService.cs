namespace WebApi.Services;

using System.Security.Claims;
using BCrypt.Net;
using Microsoft.Extensions.Options;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Users;

public interface IUserService
{
    bool Create(User user);
    AuthenticateResponse Authenticate(AuthenticateRequest model);
    AuthenticateResponse RefreshToken(string AccessToken, string refreshToken);

    // void RevokeToken(string token, string ipAddress);
    IEnumerable<User> GetAll();
    User GetById(int id);
}

public class UserService : IUserService
{
    private DataContext _context;
    private IJwtUtils _jwtUtils;
    private readonly AppSettings _appSettings;

    public UserService(DataContext context, IJwtUtils jwtUtils, IOptions<AppSettings> appSettings)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _appSettings = appSettings.Value;
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var user = _context.Users.SingleOrDefault(x => x.UserName == model.Username);

        if (user == null || !BCrypt.Verify(model.Password, user.Password))
            throw new AppException("Username or password is incorrect");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        };
        var jwtToken = _jwtUtils.GenerateJwtToken(claims);
        var refreshToken = _jwtUtils.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.TokenExpires = DateTime.Now.AddMinutes(_appSettings.RefreshTokenTTL);

        _context.Update(user);
        _context.SaveChanges();

        return new AuthenticateResponse(jwtToken, refreshToken);
    }

    public bool Create(User user)
    {
        user.Password = BCrypt.HashPassword(user.Password);
        _context.Users.Add(user);
        return _context.SaveChanges() == 1;
    }

    public AuthenticateResponse RefreshToken(string accessToken, string refreshToken)
    {
        var user = getUserByRefreshToken(refreshToken);
        var newRefreshToken = _jwtUtils.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        _context.Update(user);
        _context.SaveChanges();

        // generate new jwt
        var claimsPrincipal=_jwtUtils.GetPrincipalFromExpiredToken(accessToken);

        var jwtToken = _jwtUtils.GenerateJwtToken(claimsPrincipal.Claims);

        return new AuthenticateResponse(jwtToken, newRefreshToken);
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users;
    }

    public User GetById(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null)
            throw new KeyNotFoundException("User not found");
        return user;
    }

    // helper methods

    private User getUserByRefreshToken(string token)
    {
        var user = _context.Users.SingleOrDefault(u => u.RefreshToken == token);

        if (user == null)
            throw new AppException("Invalid token");

        if (user.TokenExpires < DateTime.Now)
            throw new AppException("Refresh Token Expired");

        return user;
    }
}
