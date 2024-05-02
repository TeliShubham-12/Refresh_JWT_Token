namespace WebApi.Models.Users;

using System.Text.Json.Serialization;
using WebApi.Entities;

public class AuthenticateResponse
{
    public string AccessToken { get; set; }

    [JsonIgnore] // refresh token is returned in http only cookie
    public string RefreshToken { get; set; }

    public AuthenticateResponse(string? accessToken, string? refreshToken)
    {
        AccessToken = accessToken ?? "";
        RefreshToken = refreshToken ?? "";
    }
}
