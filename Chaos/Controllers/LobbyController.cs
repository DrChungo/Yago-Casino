using Chaos.Api.Interface;
using Chaos.Api.Models;
using Chaos.Api.RequestEntity;
using Chaos.Api.ResponseEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chaos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LobbyController : ControllerBase
    {
        public readonly ILobbyService _lobbyService;

        public LobbyController(ILobbyService lobbyService)
        {
            _lobbyService = lobbyService;
        }

        [HttpGet("select/{idLobby}")]
        public ActionResult SelectLobby(Guid idLobby)
        {
            Lobby lobby = _lobbyService.GetLobbyById(idLobby);
            if (lobby != null) return Ok(lobby);
            return NotFound("This Lobby doesn't exist");
        }

        [HttpGet("AllLobbies")]
        public ActionResult GetAllLobbies()
        {
            List<Lobby> AllLobies = _lobbyService.GetAllLobbies();

            return Ok(AllLobies);
        }

        [HttpPost("create")]
        public IActionResult CreateLobby()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized("Token inválido o expirado.");

            var userId = new UserCreatingLobby { Id = Guid.Parse(userIdString) };
            var lobby = _lobbyService.CreateLobby(userId.Id);
            return Ok(lobby);
        }

        
        [HttpPost("join")]
        [Authorize]
        public ActionResult JoinLobby([FromBody] string lobbyCode)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized("Token inválido o expirado.");

            Guid userId = Guid.Parse(userIdString);

            try
            {
                Lobby lobby = _lobbyService.AddPlayerToLobby(lobbyCode, userId);
                return Ok(lobby);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Kick/Player")]
        [Authorize]
        public ActionResult KickPlayerFromLobby([FromBody]PlayerLobby playerKickLobby)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized("Token inválido o expirado.");

            Guid requesterUserId = Guid.Parse(userIdString);

            if (!_lobbyService.CheckUsersLobby(playerKickLobby))
                return Unauthorized("This player is not in the lobby");

            try
            {
                Lobby lobby = _lobbyService.KickPlayerLobby(playerKickLobby, requesterUserId);
                return Ok(lobby);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("Remove")]
        public ActionResult RemoveLobby([FromBody]Guid lobbyId)
        {
            bool removed = _lobbyService.RemoveLobby(lobbyId);

            if (!removed) return NotFound("Lobby no encontrado");

            return Ok("Lobby eliminado correctamente");
        }

        //nuevos para los users, añadido el 01/06/26
        [HttpGet("{lobbyId}/players")]
        [Authorize]
        public ActionResult GetPlayersInLobby(Guid lobbyId)
        {
            try
            {
                var players = _lobbyService.GetPlayersByLobby(lobbyId);
                if (players == null || !players.Any())
                    return NotFound("No players found in this lobby.");
                return Ok(players);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("leave")]
        [Authorize]
        public ActionResult LeaveLobby([FromQuery] Guid lobbyId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized("Token inválido o expirado.");

            Guid userId = Guid.Parse(userIdString);

            try
            {
                Lobby lobby = _lobbyService.LeaveFromLobby(lobbyId, userId);
                return Ok(lobby);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
