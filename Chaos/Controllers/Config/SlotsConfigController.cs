using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity.Config.SlotGame;
using Microsoft.AspNetCore.Mvc;

namespace Chaos.Api.Controllers.Config
{

    [ApiController]
    [Route("api/configs/slot-game")]
    public class SlotsConfigController : ControllerBase
    {

        private readonly ISlotGameConfigService _service;

        public SlotsConfigController(ISlotGameConfigService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all slot game configurations.
        /// </summary>
        /// <returns>A list of slot game configurations.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var configs = await _service.GetAllAsync();
            return Ok(configs);
        }
        /// <summary>
        /// Gets a specific slot game configuration by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the slot game configuration.</param>
        /// <returns>The slot game configuration if found; otherwise, a not found response.</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var config = await _service.GetByIdAsync(id);
            if (config == null) return NotFound(new { message = $"SlotGameConfig with id {id} not found." });
            return Ok(config);

        }

        /// <summary>
        /// Posts a new slot game configuration for a specific game identified by its unique identifier.
        /// </summary>
        /// <param name="request">The request object containing the details of the slot game configuration to create.</param>
        /// <returns>The created slot game configuration.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateSlotGameConfigRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing slot game configuration identified by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the slot game configuration to update.</param>
        /// <param name="request">The request object containing the updated details of the slot game configuration.</param>
        /// <returns>The updated slot game configuration if found; otherwise, a not found response.</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSlotGameConfigRequest request)
        {
            var config = await _service.UpdateAsync(id, request);
            if (config == null) return NotFound(new { message = $"SlotGameConfig with id {id} not found." });
            return Ok(config);
        }

        /// <summary>
        /// Deletes a slot game configuration identified by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the slot game configuration to delete.</param>
        /// <returns>A response indicating the result of the delete operation.</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = $"SlotGameConfig with id {id} not found." });
            return NoContent();
        }
 
    }
}

