namespace WebApi.Models.Users;

using System.ComponentModel.DataAnnotations;

public class AuthenticateRequest
{
    public required string Username { get; set; }

    public required string Password { get; set; }
}
