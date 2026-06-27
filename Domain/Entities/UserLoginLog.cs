using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CODE81_Assessment.Domain.Entities
{
    public class UserLoginLog
    {
        [Key]
        public int Id { get; set; }

        public DateTimeOffset LoginTime { get; set; } = DateTimeOffset.UtcNow;

        public bool IsSuccess { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        public string Action { get; set; } = "Login";

        [Required, ForeignKey("User")]
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
    }
}
