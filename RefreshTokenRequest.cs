// RefreshToken.cs
using System.ComponentModel.DataAnnotations;

namespace Api.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Token { get; set; }

        public virtual User User { get; set; }
    }
}
