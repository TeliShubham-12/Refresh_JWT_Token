namespace WebApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Entities;
using WebApi.Models.Users;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        AuthenticateResponse? response = _userService.Authenticate(model);
        setTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public IActionResult RefreshToken(RefreshTokenRequest request)
    {
        Console.WriteLine("cookie "+ Request.Cookies["refreshToken"]);
        var refreshToken = Request.Cookies["refreshToken"] ?? "";
        var response = _userService.RefreshToken(request.AccessToken, refreshToken);
        setTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        return Ok(users);
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Create(User user)
    {
        var result = _userService.Create(user);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _userService.GetById(id);
        return Ok(user);
    }

    // helper methods

    private void setTokenCookie(string token)
    {
        // append cookie with refresh token to the http response
        var cookieOptions = new CookieOptions
        {

            // HttpOnly = true,
            IsEssential=true,
            Expires = DateTime.UtcNow.AddDays(1),
            SameSite = SameSiteMode.None,
            Secure = true
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
}
