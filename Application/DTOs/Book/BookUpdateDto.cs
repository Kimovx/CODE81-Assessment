using CODE81_Assessment.Domain.Enums;

namespace CODE81_Assessment.Application.DTOs.Book
{
    public class BookUpdateDto
    {
        public string? Title { get; set; }

        public string? ISBN { get; set; }

        public int? PublicationYear { get; set; }

        public string? Language { get; set; }

        public string? Edition { get; set; }

        public string? Summary { get; set; }

        public BookStatus? Status { get; set; }

        public string? CoverImageUrl { get; set; }

        public int? PublisherId { get; set; }

        public List<int>? AuthorIds { get; set; }

        public List<int>? CategoryIds { get; set; }
    }
}
