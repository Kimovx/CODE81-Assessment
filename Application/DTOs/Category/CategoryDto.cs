namespace CODE81_Assessment.Application.DTOs.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        public string? ParentCategoryName { get; set; }
    }
}
