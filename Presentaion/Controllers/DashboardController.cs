using CODE81_Assessment.Application.DTOs.Dashboard;
using CODE81_Assessment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CODE81_Assessment.Presentaion.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(IDashboardService dashboardService) : ControllerBase
    {
        private readonly IDashboardService _dashboardService = dashboardService;

        [HttpGet("kpis")]
        public async Task<ActionResult<DashboardKpisDto>> GetKpis()
            => Ok(await _dashboardService.GetKpisAsync());

        [HttpGet("monthly-activity")]
        public async Task<ActionResult<IEnumerable<MonthlyActivityDto>>> GetMonthlyActivity(
            [FromQuery] int months = 12)
            => Ok(await _dashboardService.GetMonthlyActivityAsync(months));

        [HttpGet("top-books")]
        public async Task<ActionResult<IEnumerable<TopBorrowedBookDto>>> GetTopBorrowedBooks(
            [FromQuery] int top = 10)
            => Ok(await _dashboardService.GetTopBorrowedBooksAsync(top));

        [HttpGet("top-members")]
        public async Task<ActionResult<IEnumerable<TopActiveMemberDto>>> GetTopActiveMembers(
            [FromQuery] int top = 10)
            => Ok(await _dashboardService.GetTopActiveMembersAsync(top));

        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<OverdueTransactionDto>>> GetOverdueTransactions()
            => Ok(await _dashboardService.GetOverdueTransactionsAsync());

        [HttpGet("recent-transactions")]
        public async Task<ActionResult<IEnumerable<RecentTransactionDto>>> GetRecentTransactions(
            [FromQuery] int top = 10)
            => Ok(await _dashboardService.GetRecentTransactionsAsync(top));
    }
}
