namespace CODE81_Assessment.Application.DTOs.Publisher
{
    public class PublisherDto
    {
        public required int Id { get; set; }

        public required string Name { get; set; }

        public string? Country { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }
    }
}
