using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.BorrowingTransaction;
using CODE81_Assessment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CODE81_Assessment.Presentaion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowingController(IBorrowingService service) : ControllerBase
    {
        private readonly IBorrowingService _service = service;

        [Authorize(Roles = "Admin,Librarian")]
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowingTransactionDto>> GetById(int id)
            => Ok(await _service.GetByIdAsync(id));

        [Authorize(Roles = "Admin,Librarian")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<BorrowingTransactionDto>>> GetAll(int page = 1, int size = 10)
            => Ok(await _service.GetAllAsync(page, size));

        [Authorize(Roles = "Admin,Librarian,Staff")]
        [HttpPost("borrow")]
        public async Task<ActionResult> Borrow([FromBody] BorrowingTransactionCreateDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _service.BorrowAsync(dto, userId);

            return Ok(result);
        }

        [Authorize(Roles = "Admin,Librarian,Staff")]
        [HttpPost("return")]
        public async Task<IActionResult> Return([FromBody] ReturnBookDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _service.ReturnAsync(dto, userId);

            return NoContent();
        }
    }
}
