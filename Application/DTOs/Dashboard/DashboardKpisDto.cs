namespace CODE81_Assessment.Application.DTOs.Dashboard
{
    public class DashboardKpisDto
    {
        #region Books
        public int TotalBooks { get; set; }
        
        public int AvailableBooks { get; set; }
        
        public int BorrowedBooks { get; set; }
        #endregion

        #region Members
        public int TotalMembers { get; set; }
        
        public int ActiveMembers { get; set; }
        
        public int SuspendedMembers { get; set; }
        
        public int ExpiredMembers { get; set; }
        #endregion

        #region Transactions
        public int TotalTransactions { get; set; }
       
        public int ActiveTransactions { get; set; }
        
        public int ReturnedTransactions { get; set; }
        
        public int OverdueTransactions { get; set; }
        
        public int LostTransactions { get; set; }
        #endregion

        #region Other Data
        public int TotalAuthors { get; set; }
        
        public int TotalCategories { get; set; }
        
        public int TotalPublishers { get; set; }
        #endregion
    }
}
