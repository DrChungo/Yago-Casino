using Chaos.Api.Interface.Config;
using Chaos.Api.ResponseEntity.Config.CoinGame;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chaos.Api.Controllers.Config
{
    /// <summary>
    /// Controlador REST para gestionar las configuraciones del juego CoinGame.
    /// Expone endpoints CRUD completos + toggle de estado activo/inactivo.
    /// Ruta base: api/CoinGame
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]  //Ruta base: api/CoinGame (toma el nombre de la clase sin "Controller")
    // [Authorize] //Autenticación JWT desactivada temporalmente para pruebas

    public class CoinGameController : ControllerBase
    {
        // Servicio que contiene toda la lógica de negocio
        // Se inyecta automáticamente por el sistema de DI de .NET
        private readonly ICoinGameService _service;

        /// <summary>
        /// Constructor — recibe el servicio mediante Inyección de Dependencias (DI).
        /// No se instancia manualmente, .NET lo resuelve solo.
        /// </summary>
        public CoinGameController(ICoinGameService service)
        {
            _service = service;
        }

  
        /// <summary>
        /// Obtiene TODAS las configuraciones de CoinGame almacenadas en la base de datos.
        /// 
        ///Usado por: La tabla principal de CoinGamePage.razor al cargar la página.
        ///Si no hay datos: 200 OK + lista vacía []
        /// </summary>
        [HttpGet("CoinFlipConfig")]
        public async Task<IActionResult> GetAll()
        {
            var configs = await _service.GetAllAsync(); // Llama al servicio para traer todos los registros
            return Ok(configs);                         // Devuelve 200 OK con la lista
        }

 
        /// <summary>
        /// Obtiene UNA configuración específica por su ID (GUID).
        /// 
        ///Usado por: CoinGameEdit.razor al cargar los datos del formulario de edición.
        ///Si no existe: 404 Not Found.
        /// </summary>
        /// <param name="id">GUID único de la configuración a buscar</param>
        [HttpGet("CoinFlipConfig/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var config = await _service.GetByIdAsync(id); // Busca la configuración por ID
            if (config == null) return NotFound();// Si no existe → 404
            return Ok(config);// Si existe → 200 OK con los datos
        }

        /// <summary>
        /// CREA una nueva configuración de CoinGame en la base de datos.
        /// 
        /// → Usado por: CoinGameCreate.razor al enviar el formulario de creación.
        /// → Body requerido: JSON con los campos de CoinGameCreate (ConfigName, GameName, IsActive...).
        /// </summary>
        /// <param name="dto">Datos de la nueva configuración recibidos en el body de la petición</param>
        [HttpPost("CoinFlipConfig")]
        public async Task<IActionResult> Create([FromBody] CoinGameCreate dto)
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
        /// Usado por: CoinGameEdit.razor al guardar los cambios del formulario.
        ///Reemplaza TODOS los campos del registro con los datos recibidos.
        /// </summary>
        /// <param name="id">GUID de la configuración a actualizar</param>
        /// <param name="dto">Nuevos datos para reemplazar la configuración actual</param>
        [HttpPut("CoinFlipConfig/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CoinGameUpdate dto)
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
        /// → Usado por: El botón Toggle de la tabla en CoinGamePage.razor.
        /// → NO modifica ningún otro campo, solo invierte el valor de IsActive.
        /// </summary>
        /// <param name="id">GUID de la configuración a activar/desactivar</param>
        [HttpPatch("CoinFlipConfig/{id}/toggle")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var result = await _service.ToggleActiveAsync(id);
            if (!result) return NotFound();

            // Retorna la config actualizada para que el frontend sepa el nuevo estado
            var updated = await _service.GetByIdAsync(id);
            return Ok(updated);
        }

        /// <summary>
        /// ELIMINA permanentemente una configuración de la base de datos por su ID.
        /// 
        ///Usado por: El botón Eliminar de la tabla en CoinGamePage.razor.
        ///Esta acción es irreversible, el registro se borra definitivamente.
        /// </summary>
        /// <param name="id">GUID de la configuración a eliminar</param>
        [HttpDelete("CoinFlipConfig/{id}")]
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
                return BadRequest(ex.Message); //Devuelve 400 con el mensaje claro
            }
        }
    }
}