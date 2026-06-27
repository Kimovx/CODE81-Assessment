namespace CODE81_Assessment.Application.DTOs.Category
{
    public class CategoryCreateDto
    {
        public required string Name { get; set; }

        public int? ParentCategoryId { get; set; }
    }
}
