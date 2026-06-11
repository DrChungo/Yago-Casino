using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity.Config.SlotGame;
using Microsoft.AspNetCore.Mvc;

namespace Chaos.Api.Controllers.Config
{
    [ApiController]
    [Route("api/config/[controller]")]
    public class SlotPayoutRuleController : ControllerBase
    {
        private readonly ISlotPayoutRuleService _service;

        public SlotPayoutRuleController(ISlotPayoutRuleService service)
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
        public async Task<IActionResult> Create([FromBody] CreateSlotPayoutRuleRequest request)
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // UPDATE
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSlotPayoutRuleRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            if (result == null) return NotFound();
            return Ok(result);
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