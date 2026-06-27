namespace CODE81_Assessment.Application.DTOs.Dashboard
{
    public class MonthlyActivityDto
    {
        public required string Month { get; set; }
        
        public int BorrowCount { get; set; }
        
        public int ReturnCount { get; set; }
        
        public int OverdueCount { get; set; }
    }
}
