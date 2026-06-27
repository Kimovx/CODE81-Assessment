using CODE81_Assessment.Domain.Enums;

namespace CODE81_Assessment.Application.DTOs.LibraryMember
{
    public class LibraryMemberDto
    {
        public int Id { get; set; }

        public required string FullName { get; set; }

        public required string Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public DateTimeOffset MembershipStartDate { get; set; }

        public DateTimeOffset? MembershipEndDate { get; set; }

        public required MemberStatus Status { get; set; }
    }
}
