namespace CODE81_Assessment.Application.DTOs.BorrowingTransaction
{
    public class BorrowingTransactionCreateDto
    {
        public required int MemberId { get; set; }

        public required int BookId { get; set; }

        public required DateTime DueDate { get; set; }

        public string? Notes { get; set; }
    }
}
