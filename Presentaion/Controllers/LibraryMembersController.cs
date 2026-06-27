using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.LibraryMember;
using CODE81_Assessment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CODE81_Assessment.Presentaion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryMembersController(ILibraryMemberService libraryMemberService) : ControllerBase
    {
        private readonly ILibraryMemberService _libraryMemberService = libraryMemberService;

        #region CRUD
        [Authorize(Roles = "Admin,Librarian,Staff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<LibraryMemberDto>> GetById(int id)
        {
            var result = await _libraryMemberService.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Librarian,Staff")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<LibraryMemberDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)

            => Ok(await _libraryMemberService.GetAllAsync(pageNumber, pageSize));

        [Authorize(Roles = "Admin,Librarian")]
        [HttpPost]
        public async Task<ActionResult<LibraryMemberDto>> Create([FromBody] LibraryMemberCreateDto dto)
        {
            var result = await _libraryMemberService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        [Authorize(Roles = "Admin,Librarian")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LibraryMemberUpdateDto dto)
        {
            await _libraryMemberService.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _libraryMemberService.DeleteAsync(id);
            return NoContent();
        }

        #endregion
    }
}
