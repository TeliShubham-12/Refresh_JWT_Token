namespace WebApi.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("Users")]
public class User
{
    [Key]
    [Column("UserId")]
    public int UserId { get; set; }

    [Column("UserName")]
    public required string UserName { get; set; }

    [Column("Password")]
    public required string Password { get; set; }

    [JsonIgnore]
    [Column("RefreshToken")]
    public string? RefreshToken { get; set; } = null;

    [JsonIgnore]
    [Column("TokenExpires")]
    public DateTime? TokenExpires { get; set; } =null;
}
