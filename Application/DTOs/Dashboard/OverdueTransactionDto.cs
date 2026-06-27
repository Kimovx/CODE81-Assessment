namespace CODE81_Assessment.Application.DTOs.Dashboard
{
    public class OverdueTransactionDto
    {
        public int TransactionId { get; set; }
        
        public int BookId { get; set; }
        
        public required string BookTitle { get; set; }
        
        public int MemberId { get; set; }
        
        public required string MemberName { get; set; }
        
        public required string MemberEmail { get; set; }
        
        public DateTimeOffset BorrowDate { get; set; }
        
        public DateTime DueDate { get; set; }
        
        public int DaysOverdue { get; set; }
    }
}
