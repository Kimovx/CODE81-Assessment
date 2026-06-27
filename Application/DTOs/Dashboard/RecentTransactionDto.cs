namespace CODE81_Assessment.Application.DTOs.Dashboard
{
    public class RecentTransactionDto
    {
        public int TransactionId { get; set; }
        
        public required string BookTitle { get; set; }
        
        public required string MemberName { get; set; }
        
        public required string Status { get; set; }
        
        public DateTimeOffset BorrowDate { get; set; }
        
        public DateTime DueDate { get; set; }
        
        public DateTimeOffset? ReturnDate { get; set; }
        
        public required string CreatedBy { get; set; }
    }
}
