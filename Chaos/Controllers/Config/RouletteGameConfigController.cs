using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity.Config.Roulette;
using Microsoft.AspNetCore.Mvc;

namespace Chaos.Api.Controllers.Config
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouletteGameConfigController : ControllerBase
    {
        private readonly IRouletteGameConfigService _service;

        public  RouletteGameConfigController(IRouletteGameConfigService service)
        {
            _service = service;
        }

        /// <summary>Obtiene TODAS las configuraciones de Roulette.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var configs = await _service.GetAllAsync();
            return Ok(configs);
        }

        /// <summary>Obtiene UNA configuración por ID.</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var config = await _service.GetByIdAsync(id);
            if (config == null) return NotFound();
            return Ok(config);
        }

        /// <summary>CREA una nueva configuración de Roulette.</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRouletteGameConfigRequest dto)
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

        /// <summary>ACTUALIZA una configuración existente.</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRouletteGameConfigRequest dto)
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

        /// <summary>Toggle ACTIVO/INACTIVO.</summary>
        [HttpPatch("{id}/toggle")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var result = await _service.ToggleActiveAsync(id);
            if (!result) return NotFound();

            var updated = await _service.GetByIdAsync(id);
            return Ok(updated);
        }

        /// <summary>ELIMINA una configuración.</summary>
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