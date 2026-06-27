using CODE81_Assessment.Application.DTOs.Dashboard;

namespace CODE81_Assessment.Application.Interfaces.Repositories
{
    public interface IDashboardRepository
    {
        #region KPIs
        Task<DashboardKpisDto> GetKpisAsync();
        #endregion

        #region Trends
        Task<IEnumerable<MonthlyActivityDto>> GetMonthlyActivityAsync(int months);
        #endregion

        #region Top Lists
        Task<IEnumerable<TopBorrowedBookDto>> GetTopBorrowedBooksAsync(int top);
        Task<IEnumerable<TopActiveMemberDto>> GetTopActiveMembersAsync(int top);
        #endregion

        #region Alerts 
        Task<IEnumerable<OverdueTransactionDto>> GetOverdueTransactionsAsync();
        #endregion

        #region Recent Activity
        Task<IEnumerable<RecentTransactionDto>> GetRecentTransactionsAsync(int top);
        #endregion
    }
}
