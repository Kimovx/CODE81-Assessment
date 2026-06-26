using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CODE81_Assessment.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string TokenHashed { get; set; }

        [Required]
        public required string TokenId { get; set; }

        [Required]
        public DateTimeOffset ExpiresAt { get; set; }

        public DateTimeOffset? RevokedAt { get; set; }

        [Required, ForeignKey("User")]
        public int UserId { get; set; }
        public AppUser User { get; set; } = default!;
    }
}
