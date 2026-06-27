using CODE81_Assessment.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CODE81_Assessment.Domain.Entities
{
    public class Category : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public required string Name { get; set; }

        [ForeignKey("ParentCategory")]
        public int? ParentCategoryId { get; set; }

        // Navigation
        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = [];

        public ICollection<Book> Books { get; set; } = [];
    }
}
