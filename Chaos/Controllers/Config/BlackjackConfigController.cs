using Chaos.Api.Interface.Config;
using Chaos.Api.ResponseEntity.Config.Blackjack;
using Chaos.Api.Service.Config;
using Microsoft.AspNetCore.Mvc;

namespace Chaos.Api.Controllers.Config
{

    /// <summary>
    /// Controlador REST para gestionar las configuraciones del juego Blackjack.
    /// Expone endpoints CRUD completos + toggle de estado activo/inactivo.
    /// Ruta base: api/Blackjack
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BlackjackConfigController : ControllerBase
    {
        private readonly IBlackjackConfigService _service;

        public BlackjackConfigController(IBlackjackConfigService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene TODAS las configuraciones de Blackjack.
        /// Usado por: BlackjackPage.razor al cargar la tabla principal.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var configs = await _service.GetAllAsync();
            return Ok(configs);
        }

        /// <summary>
        /// Obtiene UNA configuración por su ID.
        /// Usado por: BlackjackEdit.razor al cargar el formulario.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var config = await _service.GetByIdAsync(id);
            if (config == null) return NotFound();
            return Ok(config);
        }

        /// <summary>
        /// CREA una nueva configuración de Blackjack.
        /// Usado por: BlackjackCreate.razor al enviar el formulario.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BlackjackCreateResponse dto)
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

        /// <summary>
        /// ACTUALIZA una configuración existente por su ID.
        /// Usado por: BlackjackEdit.razor al guardar cambios.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BlackjackUpdateResponse dto)
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

        /// <summary>
        /// Toggle ACTIVO/INACTIVO de una configuración.
        /// Usado por: El botón Toggle de la tabla en BlackjackPage.razor.
        /// </summary>
        [HttpPatch("{id}/toggle")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var result = await _service.ToggleActiveAsync(id);
            if (!result) return NotFound();

            var updated = await _service.GetByIdAsync(id);
            return Ok(updated);
        }

        /// <summary>
        /// ELIMINA permanentemente una configuración por su ID.
        /// Usado por: El botón Eliminar de la tabla en BlackjackPage.razor.
        /// </summary>
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

