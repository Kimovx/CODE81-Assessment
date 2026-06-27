using CODE81_Assessment.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CODE81_Assessment.Domain.Entities
{
    public class BorrowingTransaction
    {
        [Key]
        public int Id { get; set; }

        // Who borrowed
        [Required]
        public required int MemberId { get; set; }
        public LibraryMember Member { get; set; } = null!;

        // What book
        [Required, ForeignKey("Book")]
        public required int BookId { get; set; }
        public Book Book { get; set; } = null!;

        // (Staff/Librarian)
        [Required, ForeignKey("CreatedBy")]
        public required int CreatedById { get; set; }
        public AppUser CreatedBy { get; set; } = null!;

        // Who processed the return
        [ForeignKey("ReturnedBy")]
        public int? ReturnedById { get; set; }
        public AppUser? ReturnedBy { get; set; }

        [Required]
        public DateTimeOffset BorrowDate { get; set; } = DateTime.UtcNow;

        [Required]
        public required DateTime DueDate { get; set; }

        public DateTimeOffset? ReturnDate { get; set; }

        public TransactionStatus Status { get; set; } = TransactionStatus.Active;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
