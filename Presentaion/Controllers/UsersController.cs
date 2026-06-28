using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.User;
using CODE81_Assessment.Application.DTOs.UserLogs;
using CODE81_Assessment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CODE81_Assessment.Presentaion.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        #region CRUD
        [HttpGet]
        public async Task<IActionResult> GetAll()
        => Ok(await _userService.GetAllAsync());

        [HttpGet("paginated")]
        public async Task<ActionResult<PaginatedResult<UserDto>>> GetAllPaginated(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)

            => Ok(await _userService.GetAllPaginatedAsync(pageNumber, pageSize));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateDto dto)
        {
            var user = await _userService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UserUpdateDto dto)
        {
            await _userService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        #endregion

        #region Activity Logs
        [HttpGet("{id}/activity-logs")]
        public async Task<ActionResult<PaginatedResult<UserLogDto>>> GetUserActivityLogs(
            int id,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var logs = await _userService.GetUserActivityLogsPaginatedAsync(id, pageNumber, pageSize);
            return Ok(logs);
        }

        [HttpGet("activity-logs")]
        public async Task<ActionResult<PaginatedResult<UserLogDto>>> GetAllActivityLogs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var logs = await _userService.GetAllLogsPaginatedAsync(pageNumber, pageSize);
            return Ok(logs);
        }
        #endregion
    }
}
