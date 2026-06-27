using CODE81_Assessment.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CODE81_Assessment.Domain.Entities
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public required string Title { get; set; }

        [Required, MaxLength(255)]
        public required string ISBN { get; set; }

        [Required]
        public required int PublicationYear { get; set; }

        [Required, MaxLength(255)]
        public required string Language { get; set; }

        [Required, MaxLength(255)]
        public required string Edition { get; set; }

        [Required]
        public required string Summary { get; set; }

        [Required]
        public required BookStatus Status { get; set; }

        public string? CoverImageUrl { get; set; }

        [Required, ForeignKey("PublisherId")]
        public required int PublisherId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        // Navigations
        public Publisher Publisher { get; set; } = null!;
        public ICollection<Author> Authors { get; set; } = [];
        public ICollection<Category> Categories { get; set; } = [];
        public ICollection<BorrowingTransaction> BorrowingTransactions { get; set; } = [];
    }
}
