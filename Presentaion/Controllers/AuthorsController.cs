using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.Author;
using CODE81_Assessment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CODE81_Assessment.Presentaion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController(IAuthorService authorService) : ControllerBase
    {
        private readonly IAuthorService _authorService = authorService;

        #region CRUD

        [Authorize(Roles = "Admin,Librarian,Staff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetById(int id)
            => Ok(await _authorService.GetByIdAsync(id));

        [Authorize(Roles = "Admin,Librarian,Staff")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<AuthorDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)

            => Ok(await _authorService.GetAllAsync(pageNumber, pageSize));

        [Authorize(Roles = "Admin,Librarian")]
        [HttpPost]
        public async Task<ActionResult<AuthorDto>> Create([FromBody] AuthorCreateDto dto)
        {
            var author = await _authorService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
        }

        [Authorize(Roles = "Admin,Librarian")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDto dto)
        {
            await _authorService.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _authorService.DeleteAsync(id);
            return NoContent();
        }

        #endregion
    }
}
