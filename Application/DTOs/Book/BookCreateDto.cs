namespace CODE81_Assessment.Application.DTOs.Book
{
    public class BookCreateDto
    {
        public required string Title { get; set; }

        public required string ISBN { get; set; }

        public int PublicationYear { get; set; }

        public required string Language { get; set; }

        public required string Edition { get; set; }

        public required string Summary { get; set; }

        public int PublisherId { get; set; }

        public IFormFile? CoverImage { get; set; }

        public List<int> AuthorIds { get; set; } = [];

        public List<int> CategoryIds { get; set; } = [];
    }
}
