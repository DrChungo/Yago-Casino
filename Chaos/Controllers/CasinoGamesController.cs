using Chaos.Api.Interface;
using Chaos.Api.Models;
using Chaos.Api.RequestEntity;
using Chaos.Api.ResponseEntity;
using Chaos.Api.ResponseEntity.RussianRoulette;
using Chaos.Infraestructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chaos.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CasinoGamesController(ICasinoGamesService casino, IUserService userService, ITokenBlackListService tokenBlackListService, IHttpContextAccessor httpContextAccessor) : ControllerBase
    {
        private readonly ICasinoGamesService _casino = casino;

        private readonly IUserService _userService = userService;

        private readonly ITokenBlackListService _tokenBlackListService = tokenBlackListService;

        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        /// <summary>
        /// Plays a slot machine game for the specified animal and removes the animal from the system if found.
        /// </summary>
        /// <returns>An array of slot machine game results if the animal is found; otherwise, a NotFound result.</returns>

        [HttpGet("Symbols")]
        public ActionResult GetSymbols() {
            List<SlotSymbol> symbols = _casino.GetSymbolsFromDB();

            return Ok(symbols);
        }
        [HttpPost("TragaPerrillas")]
        public async Task< ActionResult> Tragaperrillas(CoinGameRequest Coin)
        {
           if (!_casino.ExistAnimal(Coin.AnimalId)) return NotFound("Where is the animal?");

            Tragaperras Game = await _casino.PlayTragaPerrillas(Coin);

            List<String[][]> SlotsMachines = _casino.ConvertToJagged(Game.ListMachines);
           return Ok(new {Tragaperras = SlotsMachines, Jackpots = Game.AllRewards, TotalReward = Game.Reward });
        }

     


        /// <summary>
        /// Plays a coin game using the specified request and returns the game result. In HeadOrTail HEAD = True / TAIL = False
        /// </summary>
        /// <param name="request">The request containing the parameters for the coin game.</param>
        /// <returns>A response containing the result of the coin game.</returns>
        [HttpPost("CoinGame")]
        public async Task<IActionResult> CoinGame(CoinFlipRequest request)
        {
            Console.WriteLine("CoinGame HIT");
            try
            {
                if (request == null || request.AnimalId == Guid.Empty)
                    return BadRequest("Invalid request");

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(userIdClaim, out var playerId))
                    return Unauthorized("Invalid user");

                string selection = request.HeadOrTail ? "HEAD" : "TAIL";

                var game = await _casino.CoinGame(
                    request.AnimalId,
                    playerId,
                    selection
                );

                return Ok(new CoinGameResponse
                {
                    AnimalName = game.AnimalName,
                    Reward = game.Reward,
                    Message = game.Message,
                    Possibility = game.Possibility,
                    Won = game.Won
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }



        /// <summary>
        /// Inicia una nueva partida de Blackjack con el animal especificado.
        /// </summary>
        /// <param name="request">Contiene el ID del animal con el que se jugará.</param>
        /// <returns>El estado inicial de la partida de Blackjack.</returns>
        [HttpPost("blackjack/start")]
        public async Task<ActionResult<BlackJackResponse>> StartBlackJack([FromBody] BlackJackRequest request)
        {

            if(request.AnimalId == Guid.Empty)
            {
                return BadRequest("Invalid request");
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized("User ID claim not found");
                }

                Guid userId = Guid.Parse(userIdClaim.Value);

                //Log del request recibido
                Console.WriteLine($"[DEBUG] Request received - AnimalId: {request.AnimalId}, UserId: {Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value)}");

                BlackJackGame game = await _casino.BlackJackGame(request.AnimalId, userId);

                Console.WriteLine($"[DEBUG] Game created successfully - GameId: {game.GameId}");

                return Ok(_casino.MapToBlackJackResponse(game));
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"[ERROR] KeyNotFoundException: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception Type: {ex.GetType().Name}");
                Console.WriteLine($"[ERROR] Message: {ex.Message}");
                Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");

                return StatusCode(500, new
                {
                    error = ex.Message,
                    type = ex.GetType().Name,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// El jugador pide una carta (Hit) en una partida de Blackjack existente.
        /// </summary>
        /// <param name="gameId">El ID de la partida de Blackjack.</param>
        /// <returns>El estado actualizado de la partida de Blackjack.</returns>

        [HttpPost("blackjack/{gameId}/hit")]
        public async Task<ActionResult<BlackJackResponse>> BlackJackHit(Guid gameId)
        {

            if (gameId == Guid.Empty)
                return BadRequest("Invalid gameId");

            try
            {
                var game = await _casino.BlackJackHit(gameId);

                return Ok(_casino.MapToBlackJackResponse(game));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// El jugador se planta (Stand) en una partida de Blackjack existente.
        /// </summary>
        /// <param name="gameId">El ID de la partida de Blackjack.</param>
        /// <returns>El estado final de la partida de Blackjack después del turno del dealer.</returns>
        [HttpPost("blackjack/{gameId}/stand")]
        public async Task<ActionResult<BlackJackResponse>> BlackJackStand(Guid gameId)
        {

            if (gameId == Guid.Empty)
                return BadRequest("Invalid gameId");

            try
            {
                var game = await _casino.BlackJackStand(gameId);

                return Ok(_casino.MapToBlackJackResponse(game));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///// <summary>
        ///// The players plays a game where only can be stand up one of them
        ///// </summary>
        ///// <param name="IdPlayers"></param>
        ///// <returns> Returns a list of the players and the round where each of them was eliminated and the winner of the game </returns>
        //[HttpPost("RussianRoullete")]
        //public ActionResult RussianRoullete(List<Guid> IdPlayers, Guid lobbyId)
        //{
        //    // 1️⃣ TOKEN Y USUARIO ACTUAL
        //    var currentToken = _httpContextAccessor.HttpContext?.Request
        //                       .Headers.Authorization.ToString()
        //                       .Replace("Bearer ", "");

        //    var currentUserId = Guid.Parse(
        //        User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"
        //    );

        //    // 2️⃣ VALIDACIONES PRIMERO ✅
        //    List<User> PlayerList = _userService.SelectedUsers(IdPlayers);

        //    if (PlayerList.Count < 2)
        //        return BadRequest("Se necesitan al menos 2 jugadores");

        //    if (PlayerList.Any(player => !player.IsActive))
        //        return Unauthorized("one or more players in this lobby are disabled to play this game");

        //    // 3️⃣ JUGAR
        //    List<RoundEliminated> ListPlayers = _casino.PlayRussianRoullete(PlayerList, lobbyId);

        //    // 4️⃣ GANADOR
        //    var ListAndWinner = new
        //    {
        //        Rounds = ListPlayers,
        //        Winner = ListPlayers.Find(p => p.RoundEliminatedPlayer == -1)!.IdEliminatedPlayer
        //    };

        //    // 5️⃣ REVOCAR TOKEN SI PERDIÓ
        //    bool currentUserLost = ListPlayers.Any(p =>
        //        p.IdEliminatedPlayer == currentUserId &&
        //        p.RoundEliminatedPlayer != -1);

        //    if (currentUserLost && !string.IsNullOrEmpty(currentToken))
        //        _tokenBlackListService.RevokeToken(currentToken);

        //    // 6️⃣ RESPUESTA
        //    return Ok(new RussianRouletteStatusResponse
        //    {
        //        LobbyId = lobbyId,
        //        WinnerId = ListAndWinner.Winner,
        //        WinnerName = PlayerList.First(p => p.Id == ListAndWinner.Winner).Name,
        //        SessionClosed = currentUserLost,                                        
        //        AlivePlayers = 1,
        //        TotalPlayers = PlayerList.Count,
        //        Status = "Finished",
        //        Players = PlayerList.Select((player, index) =>
        //        {
        //            var roundInfo = ListPlayers.FirstOrDefault(r => r.IdEliminatedPlayer == player.Id);
        //            bool isWinner = roundInfo?.RoundEliminatedPlayer == -1;

        //            return new PlayerStatusResponse
        //            {
        //                PlayerId = player.Id,
        //                Name = player.Name,
        //                IsAlive = isWinner,
        //                IsBot = false,
        //                TurnOrder = index + 1,
        //                IsWinner = isWinner
        //            };
        //        }).ToList()
        //    });
        //}



        /// <summary>
        /// Obtiene el estado actual de una partida de Blackjack.
        /// </summary>
        /// <param name="gameId">El ID de la partida de Blackjack.</param>
        /// <returns>El estado actual de la partida de Blackjack.</returns>
        [HttpGet("blackjack/{gameId}")]
        public ActionResult<BlackJackResponse> GetBlackJackGame(Guid gameId)
        {
            try
            {
                BlackJackGame game = _casino.BlackJackGetGame(gameId);
                return Ok(_casino.MapToBlackJackResponse(game));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Inicia una nueva partida de Higher or Lower.
        /// </summary>
        /// <param name="request">Contiene el ID del animal con el que se jugará.</param>
        /// <returns>El estado inicial de la partida de Higher or Lower.</returns>
        [HttpPost("higherlower/start")]
        public async Task<ActionResult<HigherLowerResponse>> StartHigherLower([FromBody] HigherLowerRequest request)
        {

            if (request.AnimalId == null)
            {
                return BadRequest("Invalid request");
            }

            try
            {
                Console.WriteLine($"[DEBUG] Starting game for AnimalId: {request.AnimalId}");
                Console.WriteLine($"[DEBUG] UserId from token: {User.FindFirst(ClaimTypes.NameIdentifier)?.Value}");

                HigherLowerGame game = await _casino.StartHigherLowerGame(request.AnimalId, Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value));
                return Ok(_casino.MapToHigherLowerResponse(game));
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"[ERROR] KeyNotFoundException: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exception Type: {ex.GetType().Name}");
                Console.WriteLine($"[ERROR] Message: {ex.Message}");
                Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");

                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// El jugador hace su elección (HIGHER o LOWER) en una partida activa.
        /// </summary>
        /// <param name="request">Contiene el GameId y la elección del jugador (HIGHER o LOWER).</param>
        /// <returns>El estado actualizado de la partida de Higher or Lower.</returns>
        [HttpPost("higherlower/play")]
        public async Task<ActionResult<HigherLowerResponse>> PlayHigherLower([FromBody] HigherLowerPlayRequest request)
        {
            try
            {
                var game = await _casino.PlayHigherLower(request);

                return Ok(_casino.MapToHigherLowerResponse(game));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Obtiene el estado actual de una partida de Higher or Lower.
        /// </summary>
        /// <param name="gameId">El ID de la partida de Higher or Lower.</param>
        /// <returns>El estado actual de la partida.</returns>
        [HttpGet("higherlower/{gameId}")]
        public ActionResult<HigherLowerResponse> GetHigherLowerGame(Guid gameId)
        {
            try
            {
                HigherLowerGame game = _casino.GetHigherLowerGame(gameId);
                return Ok(_casino.MapToHigherLowerResponse(game));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// El jugador retira su recompensa acumulada en una partida de Higher or Lower.
        /// </summary>
        /// <param name="gameId">El ID de la partida de Higher or Lower.</param>
        /// <returns>El estado final de la partida.</returns>
        [HttpPost("higherlower/{gameId}/cashout")]
        public async Task<ActionResult<HigherLowerResponse>> CashOutHigherLower(Guid gameId)
        {
            try
            {
                var game = await _casino.CashOutHigherLower(gameId);

                return Ok(_casino.MapToHigherLowerResponse(game));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error.");
            }
        }


        /// <summary>
        /// You can Play European Roulette Beting in color number, specific numbers, the first half, Dozen ,row... etc
        /// </summary>
        /// <param name="Bet"></param>
        /// <returns>CashBack, the number of the roulette ball position and the bet  </returns>
        //Pendiente a entender...
        [HttpPost("EuropeanRoullete")]
        public async Task <ActionResult> EuropeanRoullete(SelectionUserEuropeanRoulette Bet)
        {
            MessageWarning Message =  _casino.CheckNotDuplicateAnimalAndAvailable(Bet, Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value));
           
            
            if (Message.Disable) return NotFound(Message);
            CashBack Score =  await _casino.PlayEuropeanRoullete(Bet, Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value));
            return Ok(Score);
        }
    }
}

    


