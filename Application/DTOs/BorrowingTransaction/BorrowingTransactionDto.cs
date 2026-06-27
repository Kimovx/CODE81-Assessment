using CODE81_Assessment.Domain.Enums;

namespace CODE81_Assessment.Application.DTOs.BorrowingTransaction
{
    public class BorrowingTransactionDto
    {
        public required int Id { get; set; }

        public int MemberId { get; set; }

        public required string MemberName { get; set; }

        public int BookId { get; set; }

        public required string BookTitle { get; set; }

        public DateTimeOffset BorrowDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTimeOffset? ReturnDate { get; set; }

        public TransactionStatus Status { get; set; }

        public string? Notes { get; set; }
    }
}
