namespace CODE81_Assessment.Application.DTOs.Publisher
{
    public class PublisherCreateDto
    {
        public required string Name { get; set; }

        public string? Country { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }
    }
}
