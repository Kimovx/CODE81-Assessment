using CODE81_Assessment.Application.DTOs.Dashboard;
using CODE81_Assessment.Application.Interfaces.Repositories;
using CODE81_Assessment.Application.Interfaces.Services;

namespace CODE81_Assessment.Application.Services
{
    public class DashboardService(IDashboardRepository dashboardRepository) : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository = dashboardRepository;

        #region KPIs
        public async Task<DashboardKpisDto> GetKpisAsync()
            => await _dashboardRepository.GetKpisAsync();
        #endregion

        #region Trends
        public async Task<IEnumerable<MonthlyActivityDto>> GetMonthlyActivityAsync(int months = 12)
            => await _dashboardRepository.GetMonthlyActivityAsync(months);
        #endregion

        #region Top Lists 
        public async Task<IEnumerable<TopBorrowedBookDto>> GetTopBorrowedBooksAsync(int top = 10)
            => await _dashboardRepository.GetTopBorrowedBooksAsync(top);

        public async Task<IEnumerable<TopActiveMemberDto>> GetTopActiveMembersAsync(int top = 10)
            => await _dashboardRepository.GetTopActiveMembersAsync(top);
        #endregion

        #region Alerts
        public async Task<IEnumerable<OverdueTransactionDto>> GetOverdueTransactionsAsync()
            => await _dashboardRepository.GetOverdueTransactionsAsync();
        #endregion

        #region Recent Activity
        public async Task<IEnumerable<RecentTransactionDto>> GetRecentTransactionsAsync(int top = 10)
           => await _dashboardRepository.GetRecentTransactionsAsync(top);
        #endregion
    }
}
