using CODE81_Assessment.Domain.Enums;

namespace CODE81_Assessment.Application.DTOs.Book
{
    public class BookDto
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required string ISBN { get; set; }

        public int PublicationYear { get; set; }

        public required string Language { get; set; }

        public required string Edition { get; set; }

        public required string Summary { get; set; }

        public BookStatus Status { get; set; }

        public string? CoverImageUrl { get; set; }

        public required string PublisherName { get; set; }

        public List<string> Authors { get; set; } = [];

        public List<string> Categories { get; set; } = [];
    }
}
