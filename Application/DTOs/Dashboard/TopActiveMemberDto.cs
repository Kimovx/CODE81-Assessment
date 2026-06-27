namespace CODE81_Assessment.Application.DTOs.Dashboard
{
    public class TopActiveMemberDto
    {
        public int MemberId { get; set; }

        public required string FullName { get; set; }

        public required string Email { get; set; }

        public int BorrowCount { get; set; }
    }
}
