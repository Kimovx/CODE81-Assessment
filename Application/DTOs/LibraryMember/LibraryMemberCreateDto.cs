namespace CODE81_Assessment.Application.DTOs.LibraryMember
{
    public class LibraryMemberCreateDto
    {
        public required string FullName { get; set; }

        public required string Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }
    }
}
