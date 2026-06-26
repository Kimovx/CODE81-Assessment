using CODE81_Assessment.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CODE81_Assessment.Domain.Entities
{
    public class LibraryMember
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public required string FullName { get; set; }

        [Required, MaxLength(255)]
        public required string Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public DateTimeOffset MembershipStartDate { get; set; } = DateTime.UtcNow;

        public DateTimeOffset? MembershipEndDate { get; set; }

        public MemberStatus Status { get; set; } = MemberStatus.Active;

        // Navigation
        public ICollection<BorrowingTransaction> BorrowingTransactions { get; set; } = [];
    }
}
