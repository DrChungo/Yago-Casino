using Chaos.Api.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Chaos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RussianRouletteControllerr: ControllerBase
    {
        private readonly IRussianRouletteServicee _russianRouletteService;

        public RussianRouletteControllerr(IRussianRouletteServicee russianRouletteService)
        {
            _russianRouletteService = russianRouletteService;
        }

        /// <summary>Inicia el juego y rellena bots si faltan jugadores</summary>
        [HttpPost("Start/{lobbyId}")]
        public async Task<ActionResult> StartGame(Guid lobbyId)
        {
            try
            {
                var result = await _russianRouletteService.StartGame(lobbyId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        /// <summary>Dispara una ronda → un turno por llamada</summary>
        [HttpPost("PlayRound/{lobbyId}")]
        public async Task<ActionResult> PlayRound(Guid lobbyId)
        {
            try
            {
                var result = await _russianRouletteService.PlayRound(lobbyId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        }

        /// <summary>Obtiene el estado actual del lobby</summary>
        [HttpGet("Status/{lobbyId}")]
        public async Task<ActionResult> GetStatus(Guid lobbyId)
        {
            try
            {
                var result = await _russianRouletteService.GetStatus(lobbyId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        }


        // =============================================nuevo añadido el 01/06/2026
        /// <summary>Obtiene el historial de rondas de un lobby</summary>
        [HttpGet("history/{lobbyId}")]
        public async Task<ActionResult> GetHistory(Guid lobbyId)
        {
            try
            {
                var history = await _russianRouletteService.GetRoundHistory(lobbyId);
                if (history == null || !history.Any())
                    return NotFound("No rounds found for this lobby.");
                return Ok(history);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

    }
}
