using CODE81_Assessment.Application.DTOs.Dashboard;

namespace CODE81_Assessment.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        #region KPIs
        Task<DashboardKpisDto> GetKpisAsync();
        #endregion

        #region Trends 
        Task<IEnumerable<MonthlyActivityDto>> GetMonthlyActivityAsync(int months = 12);
        #endregion

        #region Top Lists 
        Task<IEnumerable<TopBorrowedBookDto>> GetTopBorrowedBooksAsync(int top = 10);
        Task<IEnumerable<TopActiveMemberDto>> GetTopActiveMembersAsync(int top = 10);
        #endregion

        #region Alerts
        Task<IEnumerable<OverdueTransactionDto>> GetOverdueTransactionsAsync();
        #endregion

        #region Recent Activity 
        Task<IEnumerable<RecentTransactionDto>> GetRecentTransactionsAsync(int top = 10);
        #endregion
    }
}
