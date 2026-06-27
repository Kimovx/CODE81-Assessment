using CODE81_Assessment.Application.Common;
using CODE81_Assessment.Application.DTOs.Publisher;
using CODE81_Assessment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CODE81_Assessment.Presentaion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController(IPublisherService publisherService) : ControllerBase
    {
        private readonly IPublisherService _publisherService = publisherService;

        #region CRUD
        [Authorize(Roles = "Admin,Librarian,Staff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<PublisherDto>> GetById(int id)
        {
            var result = await _publisherService.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Librarian,Staff")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<PublisherDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)

            => Ok(await _publisherService.GetAllAsync(pageNumber, pageSize));

        [Authorize(Roles = "Admin,Librarian")]
        [HttpPost]
        public async Task<ActionResult<PublisherDto>> Create([FromBody] PublisherCreateDto dto)
        {
            var result = await _publisherService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        [Authorize(Roles = "Admin,Librarian")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PublisherUpdateDto dto)
        {
            await _publisherService.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _publisherService.DeleteAsync(id);
            return NoContent();
        }

        #endregion
    }
}
