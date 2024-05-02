namespace WebApi.Models.Users;

public class RefreshTokenRequest
{
    public required string AccessToken { get; set; }
}