using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity.Config.HigherLower;
using Microsoft.AspNetCore.Mvc;

namespace Chaos.Api.Controllers.Config
{
    /// <summary>
    /// Controlador REST para gestionar las configuraciones del juego HigherLowerGame.
    /// Expone endpoints CRUD completos + toggle de estado activo/inactivo.
    /// Ruta base: api/HigherLowerGame
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HigherLowerGameController : ControllerBase
    {
        private readonly IHigherLowerGameService _service;

        public HigherLowerGameController(IHigherLowerGameService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene TODAS las configuraciones de HigherLowerGame.
        /// Usado por: La tabla principal de HigherLowerGamePage.razor al cargar la página.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var configs = await _service.GetAllAsync();
            return Ok(configs);
        }

        /// <summary>
        /// Obtiene UNA configuración específica por su ID (GUID).
        /// Usado por: HigherLowerGameEdit.razor al cargar los datos del formulario.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var config = await _service.GetByIdAsync(id);
            if (config == null) return NotFound();
            return Ok(config);
        }

        /// <summary>
        /// CREA una nueva configuración de HigherLowerGame.
        /// Usado por: HigherLowerGameCreate.razor al enviar el formulario.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HigherLowerCreateRequest dto)
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
        /// ACTUALIZA completamente una configuración existente por su ID.
        /// Usado por: HigherLowerGameEdit.razor al guardar los cambios.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] HigherLowerUpdateRequest dto)
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
        /// Cambia el estado ACTIVO/INACTIVO de una configuración (toggle).
        /// Usado por: El botón Toggle de la tabla en HigherLowerGamePage.razor.
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
        /// Usado por: El botón Eliminar de la tabla en HigherLowerGamePage.razor.
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