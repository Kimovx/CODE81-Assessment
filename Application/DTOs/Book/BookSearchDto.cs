namespace CODE81_Assessment.Application.DTOs.Book
{
    public class BookSearchDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Category { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
