namespace CODE81_Assessment.Application.DTOs.Dashboard
{
    public class TopBorrowedBookDto
    {
        public int BookId { get; set; }

        public required string Title { get; set; }

        public required string ISBN { get; set; }

        public int BorrowCount { get; set; }

        public List<string> Authors { get; set; } = [];
    }
}
