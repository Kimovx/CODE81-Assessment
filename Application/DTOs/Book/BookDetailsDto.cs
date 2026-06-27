namespace CODE81_Assessment.Application.DTOs.Book
{
    public class BookDetailsDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string ISBN { get; set; } = null!;

        public int PublicationYear { get; set; }

        public string Language { get; set; } = null!;

        public string Edition { get; set; } = null!;

        public string Summary { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string? CoverImageUrl { get; set; }

        public string PublisherName { get; set; } = null!;

        public List<string> Authors { get; set; } = [];

        public List<string> Categories { get; set; } = [];
    }
}
