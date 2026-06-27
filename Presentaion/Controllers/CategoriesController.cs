using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.Category;
using CODE81_Assessment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CODE81_Assessment.Presentaion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;

        #region CRUD
        [Authorize(Roles = "Admin,Librarian,Staff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
            => Ok(await _categoryService.GetByIdAsync(id));

        [Authorize(Roles = "Admin,Librarian,Staff")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<CategoryDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)

            => Ok(await _categoryService.GetAllAsync(pageNumber, pageSize));

        [Authorize(Roles = "Admin,Librarian")]
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryCreateDto dto)
        {
            var result = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize(Roles = "Admin,Librarian")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
        {
            await _categoryService.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();
        }
        #endregion
    }
}
