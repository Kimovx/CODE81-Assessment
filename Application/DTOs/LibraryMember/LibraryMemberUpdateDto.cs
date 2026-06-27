using CODE81_Assessment.Domain.Enums;

namespace CODE81_Assessment.Application.DTOs.LibraryMember
{
    public class LibraryMemberUpdateDto
    {
        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public DateTimeOffset? MembershipEndDate { get; set; }

        public MemberStatus? Status { get; set; }
    }
}
