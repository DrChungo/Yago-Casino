using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity.Config.RussianRoulette;
using Microsoft.AspNetCore.Mvc;

namespace Chaos.Api.Controllers.Config
{

        [ApiController]
        [Route("api/[controller]")]
        public class RussianRouletteController : ControllerBase
        {


            private readonly IRussianRouletteService _service;

            public RussianRouletteController(IRussianRouletteService service)
            {
                _service = service;
            }
            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var configs = await _service.GetAllAsync();
                return Ok(configs);
            }
            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(Guid id)
            {
                var config = await _service.GetByIdAsync(id);
                if (config == null) return NotFound();
                return Ok(config);
            }
            [HttpPost]
            public async Task<IActionResult> Create([FromBody] RussianRouletteCreateRequest dto)
            {
                try
                {
                    var created = await _service.CreateAsync(dto);
                    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }

            [HttpPut("{id}")]
            public async Task<IActionResult> Update(Guid id, [FromBody] RussianRouletteUpdateRequest dto)
            {
                try
                {
                    var updated = await _service.UpdateAsync(id, dto);
                    if (updated == null) return NotFound();
                    return Ok(updated);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);


                }

            }
            [HttpPatch("{id}/toggle")]
            public async Task<IActionResult> Toggle(Guid id)
            {


                var result = await _service.ToggleActiveAsync(id);
                if (!result) return NotFound();
                var updated = await _service.GetByIdAsync(id);

                return Ok(updated);

            }
            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(Guid id)
            {
                try
                {
                    var result = await _service.DeleteAsync(id);
                    if (!result) return NotFound("Configuración no encontrada");
                    return Ok("Configuración eliminada correctamente");
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(ex.Message);
                }

            }
        }
    }

