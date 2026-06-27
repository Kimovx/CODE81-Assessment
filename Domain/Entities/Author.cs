using System.ComponentModel.DataAnnotations;

namespace CODE81_Assessment.Domain.Entities
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public required string Name { get; set; }

        // Navigation
        public ICollection<Book> Books { get; set; } = [];
    }
}
