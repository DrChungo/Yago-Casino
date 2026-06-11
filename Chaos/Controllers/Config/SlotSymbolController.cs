using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity.Config.SlotGame;
using Microsoft.AspNetCore.Mvc;

namespace Chaos.Api.Controllers.Config
{
    [ApiController]
    [Route("api/config/[controller]")]
    public class SlotSymbolController : ControllerBase
    {
        private readonly ISlotSymbolService _service;

        public SlotSymbolController(ISlotSymbolService service)
        {
            _service = service;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // GET BY ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSlotSymbolRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // UPDATE
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSlotSymbolRequest request)
        {
            try
            {
                var result = await _service.UpdateAsync(id, request);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        // UPDATE STATUS
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] bool isActive)
        {
            var result = await _service.UpdateStatusAsync(id, isActive);
            if (!result) return NotFound();
            return Ok(new { message = $"Status updated to {isActive}" });
        }
    }
}