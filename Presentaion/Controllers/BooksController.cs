using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.Book;
using CODE81_Assessment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CODE81_Assessment.Presentaion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController(IBookService bookService) : ControllerBase
    {
        private readonly IBookService _bookService = bookService;

        #region CRUD
        [Authorize(Roles = "Admin,Librarian,Staff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetById(int id)
        {
            var result = await _bookService.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Librarian,Staff")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<BookDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)

            => Ok(await _bookService.GetAllAsync(pageNumber, pageSize));

        [Authorize(Roles = "Admin,Librarian")]
        [HttpPost]
        public async Task<ActionResult<BookDto>> Create([FromForm] BookCreateDto dto)
        {
            var book = await _bookService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { book.Id }, book);
        }

        [Authorize(Roles = "Admin,Librarian")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDto dto)
        {
            await _bookService.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _bookService.DeleteAsync(id);
            return NoContent();
        }

        #endregion

        #region Other Operations
        [Authorize(Roles = "Admin,Librarian,Staff")]
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedResult<BookDto>>> Search([FromQuery] BookSearchDto dto)
            => Ok(await _bookService.SearchAsync(dto));

        [Authorize(Roles = "Admin,Librarian,Staff")]
        [HttpGet("by-status")]
        public async Task<ActionResult<PaginatedResult<BookDetailsDto>>> GetByStatus([FromQuery] BookByStatusDto dto)
            => Ok(await _bookService.GetByStatusAsync(dto));
        #endregion
    }
}
