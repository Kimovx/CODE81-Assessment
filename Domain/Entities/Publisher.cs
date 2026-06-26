using System.ComponentModel.DataAnnotations;

namespace CODE81_Assessment.Domain.Entities
{
    public class Publisher
    {
        [Key]
        public required int Id { get; set; }

        [Required, MaxLength(255)]
        public required string Name { get; set; }

        [MaxLength(255)]
        public string? Country { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? Phone { get; set; }

        // Navigation
        public ICollection<Book> Books { get; set; } = [];
    }
}
