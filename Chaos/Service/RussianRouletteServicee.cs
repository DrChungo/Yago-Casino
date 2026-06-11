using Chaos.Api.Interface;
using Chaos.Api.ResponseEntity.RussianRoulette;
using Chaos.Infraestructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service
{
    public class RussianRouletteServicee: IRussianRouletteServicee
    {
        private readonly CasinoDBContext _dbContext;
        private readonly IGameSessionervice _gameSessionService;
        private readonly Random _random = Random.Shared;

        public RussianRouletteServicee(
            CasinoDBContext dbContext,
            IGameSessionervice gameSessionService)
        {
            _dbContext = dbContext;
            _gameSessionService = gameSessionService;
        }

        // =============================================
        // START GAME
        // =============================================
        public async Task<RussianRouletteStatusResponse> StartGame(Guid lobbyId)
        {
            //Cargar lobby con config
            var lobby = await _dbContext.RussianRouletteLobbies
                .Include(l => l.RussianRouletteGameConfig)
                .FirstOrDefaultAsync(l => l.Id == lobbyId)
                ?? throw new KeyNotFoundException($"Lobby {lobbyId} not found");

            if (lobby.Status != "Waiting")
                throw new InvalidOperationException($"Lobby is already {lobby.Status}");

            // Cargar jugadores reales actuales
            var currentPlayers = await _dbContext.RussianRoulettePlayers
                .Where(p => p.LobbyId == lobbyId)
                .ToListAsync();

            int config_MaxPlayers = lobby.RussianRouletteGameConfig.MaxPlayers;
            int config_MinPlayers = lobby.RussianRouletteGameConfig.MinPlayers;
            bool allowBots = lobby.RussianRouletteGameConfig.AllowBots;

            //Validar mínimo de jugadores
            if (currentPlayers.Count < config_MinPlayers && !allowBots)
                throw new InvalidOperationException(
                    $"Need at least {config_MinPlayers} players to start. Current: {currentPlayers.Count}");

            //Rellenar con bots si faltan jugadores
            if (allowBots && currentPlayers.Count < config_MaxPlayers)
            {
                int botsNeeded = config_MaxPlayers - currentPlayers.Count;
                string[] botNames = {
                    "Bot_soto", "Bot_hang", "Bot_ponce",
                    "Bot_Vaquer", "Bot_puerto", "Bot_flores"
                };

                for (int i = 0; i < botsNeeded; i++)
                {
                    var bot = new RussianRoulettePlayer
                    {
                        Id = Guid.NewGuid(),
                        IsBot = true,
                        BotName = botNames[i % botNames.Length],
                        IsAlive = true,
                        IsWinner = false,
                        JoinedAt = DateTime.UtcNow.ToString(),
                        UserId = null,
                        LobbyId = lobbyId
                    };
                    _dbContext.RussianRoulettePlayers.Add(bot);
                    currentPlayers.Add(bot);
                }
            }

            //Asignar TurnOrder aleatorio
            var shuffled = currentPlayers.OrderBy(_ => _random.Next()).ToList();
            for (int i = 0; i < shuffled.Count; i++)
            {
                shuffled[i].TurnOrder = i + 1;
            }

            //Calcular PrizePool: FixedPrizePool de la config
            lobby.CurrentPrizePool = lobby.RussianRouletteGameConfig.FixedPrizePool;

            //Cambiar estado del lobby
            lobby.Status = "InProgress";
            lobby.StartedAt = DateTime.UtcNow.ToString();

            await _dbContext.SaveChangesAsync();

            return await BuildStatusResponse(lobby, currentPlayers);
        }

        // =============================================
        // PLAY ROUND
        // =============================================
        public async Task<RoundResultResponse> PlayRound(Guid lobbyId)
        {
            // Cargar lobby
            var lobby = await _dbContext.RussianRouletteLobbies
                .Include(l => l.RussianRouletteGameConfig)
                .FirstOrDefaultAsync(l => l.Id == lobbyId)
                ?? throw new KeyNotFoundException($"Lobby {lobbyId} not found");

            if (lobby.Status != "InProgress")
                throw new InvalidOperationException($"Game is not in progress. Status: {lobby.Status}");

            //Cargar jugadores vivos ordenados por turno
            var alivePlayers = await _dbContext.RussianRoulettePlayers
                .Where(p => p.LobbyId == lobbyId && p.IsAlive && (p.IsBot || p.UserId != null))
                .OrderBy(p => p.TurnOrder)
                .ToListAsync();

            if (alivePlayers.Count <= 1)
                throw new InvalidOperationException("Game already has a winner!");

            //Determinar ronda actual
            int currentRound = await _dbContext.RussianRouletteRounds
                .Where(r => r.LobbyId == lobbyId)
                .CountAsync() + 1;

            //Determinar jugador en turno (rotación circular)
            int turnIndex = (currentRound - 1) % alivePlayers.Count;
            var currentPlayer = alivePlayers[turnIndex];

            //Calcular si hay bala
            int totalChambers = lobby.RussianRouletteGameConfig.TotalChambers;
            int bulletCount = lobby.RussianRouletteGameConfig.BulletCount;
            bool wasBullet = _random.Next(totalChambers) < bulletCount;

            // Guardar la ronda
            var round = new RussianRouletteRound
            {
                Id = Guid.NewGuid(),
                RoundNumber = currentRound,
                WasBullet = wasBullet,
                PlayedAt = DateTime.UtcNow.ToString(),
                LobbyId = lobbyId,
                PlayerId = currentPlayer.Id
            };
            _dbContext.RussianRouletteRounds.Add(round);

            string playerName = currentPlayer.IsBot
                ? currentPlayer.BotName
                : (await _dbContext.Users.FindAsync(currentPlayer.UserId))?.Name ?? "Unknown";

            var result = new RoundResultResponse
            {
                RoundNumber = currentRound,
                PlayerId = currentPlayer.Id,
                PlayerName = playerName,
                WasBullet = wasBullet,
                IsBot = currentPlayer.IsBot,
                GameFinished = false
            };

            //Si hay bala → eliminar jugador
            if (wasBullet)
            {
                currentPlayer.IsAlive = false;
                currentPlayer.EliminatedAt = DateTime.UtcNow.ToString();

                // Si es jugador real -> desactivar cuenta (IsActive = false)
                if (!currentPlayer.IsBot && currentPlayer.UserId.HasValue)
                {
                    var user = await _dbContext.Users.FindAsync(currentPlayer.UserId.Value);
                    if (user != null)
                    {
                        user.IsActive = false;
                        _dbContext.Users.Update(user);
                    }
                }

                result.Message = wasBullet && !currentPlayer.IsBot
                    ? $"💀 BANG! {playerName} was shot! Their account has been desactivated."
                    : $"💀 BANG! Bot {playerName} was eliminated!";

                //Recalcular vivos después de eliminar
                var remainingAlive = alivePlayers.Where(p => p.Id != currentPlayer.Id) .ToList();

                //ver si quedan humanos vivos
                var humansAlive = remainingAlive
               .Where(p => !p.IsBot && p.UserId.HasValue)
               .ToList();

                //miramos si quedan vivos y si queda 1 el ganador es el que queda vivo
                if (remainingAlive.Count == 1)
                {
                    await FinishGame(lobby, remainingAlive.First(), result);
                }
                //Condición 2: No quedan humanos pero sí bots → terminar igualmente
                else if (humansAlive.Count == 0)
                {
                    var botWinner = remainingAlive.First(); // primer bot vivo
                    await FinishGame(lobby, botWinner, result);
                }


            }
            else
            {
                result.Message = $"🔫 Click! {playerName} survived this round!";
            }

            await _dbContext.SaveChangesAsync();
            return result;
        }


        // =============================================
        // HELPER: FINISH GAME
        // =============================================
        private async Task FinishGame(
            RussianRouletteLobby lobby,
            RussianRoulettePlayer winner,
            RoundResultResponse result)
        {
            winner.IsWinner = true;

            string winnerName = winner.IsBot
                ? winner.BotName
                : (await _dbContext.Users.FindAsync(winner.UserId))?.Name ?? "Unknown";

            // Solo dar premio si el ganador es humano
            if (!winner.IsBot && winner.UserId.HasValue)
            {
                var winnerUser = await _dbContext.Users.FindAsync(winner.UserId.Value);
                if (winnerUser != null)
                {
                    winnerUser.Wallet += lobby.CurrentPrizePool;
                    _dbContext.Users.Update(winnerUser);
                }
            }

            // Actualizar lobby
            lobby.Status = "Finished";
            lobby.WinnerId = winner.IsBot ? null : winner.UserId; // ✅ null si gana un bot
            lobby.FinishedAt = DateTime.UtcNow.ToString();

            // Crear GameSessions para jugadores humanos
            await CreateGameSessionsForAllPlayers(lobby, winner);

            result.GameFinished = true;
            result.WinnerId = winner.IsBot ? null : winner.UserId;
            result.WinnerName = winnerName;
            result.PrizePool = lobby.CurrentPrizePool;
            result.Message += winner.IsBot
                ? $" 🤖 No human survivors! Bot {winnerName} wins. No prize awarded."
                : $" 🏆 {winnerName} wins {lobby.CurrentPrizePool}!";
        }


        // =============================================
        // CREATE GAME SESSIONS FOR ALL PLAYERS
        // =============================================
        private async Task CreateGameSessionsForAllPlayers(
            RussianRouletteLobby lobby,
            //List<RussianRoulettePlayer> allPlayers,
            RussianRoulettePlayer winner)
        {
            var allPlayers = await _dbContext.RussianRoulettePlayers.Where(p => p.LobbyId == lobby.Id).ToListAsync();

            // Buscar el Game de tipo RussianRoulette
            var gameEntity = await _dbContext.Games
                .FirstOrDefaultAsync(g => g.GameType.ToLower() == "russianroulette"
                                       || g.GameType.ToLower() == "russian roulette")
                ?? throw new Exception("Game type 'RussianRoulette' not found in Games table");

            // GameSession para el ganador → vinculada al lobby
            if (!winner.IsBot && winner.UserId.HasValue)
            {
                var winnerGameSession = new GameSession
                {
                    Id = Guid.NewGuid(),
                    GameId = gameEntity.Id,
                    Result = "WIN",
                    MoneyEarned = lobby.CurrentPrizePool,
                    Multiplier = 1,
                    PlayedAt = DateTime.UtcNow.ToString(),
                    UserId = winner.UserId.Value,
                    AnimalId = null
                };
                await _gameSessionService.AddGameSession(winnerGameSession);

                // Vincular GameSession al lobby
                lobby.GameSessionId = winnerGameSession.Id;
            }


            // GameSessions para los eliminados
            var losers = allPlayers.Where(p => p.Id != winner.Id && !p.IsBot && p.UserId.HasValue).ToList();

            foreach (var loser in losers)
            {
                if (loser.IsBot) continue; // Los bots no tienen GameSession

                var loserGameSession = new GameSession
                {
                    Id = Guid.NewGuid(),
                    GameId = gameEntity.Id,
                    Result = "LOSE",
                    MoneyEarned = 0,
                    Multiplier = 1,
                    PlayedAt = DateTime.UtcNow.ToString(),
                    UserId = loser.UserId!.Value,
                    AnimalId = null
                };
                await _gameSessionService.AddGameSession(loserGameSession);
            }
        }

        // =============================================
        // GET STATUS
        // =============================================
        public async Task<RussianRouletteStatusResponse> GetStatus(Guid lobbyId)
        {
            var lobby = await _dbContext.RussianRouletteLobbies
                .Include(l => l.RussianRouletteGameConfig)
                .FirstOrDefaultAsync(l => l.Id == lobbyId)
                ?? throw new KeyNotFoundException($"Lobby {lobbyId} not found");

            var players = await _dbContext.RussianRoulettePlayers
                .Where(p => p.LobbyId == lobbyId)
                .ToListAsync();

            return await BuildStatusResponse(lobby, players);
        }

        // =============================================
        // HELPER: BUILD STATUS RESPONSE
        // =============================================
        private async Task<RussianRouletteStatusResponse> BuildStatusResponse(
            RussianRouletteLobby lobby,
            List<RussianRoulettePlayer> players)
        {
            var playerResponses = new List<PlayerStatusResponse>();

            foreach (var p in players)
            {
                string name = p.IsBot
                    ? p.BotName
                    : (await _dbContext.Users.FindAsync(p.UserId))?.Name ?? "Unknown";

                playerResponses.Add(new PlayerStatusResponse
                {
                    PlayerId = p.Id,
                    Name = name,
                    IsAlive = p.IsAlive,
                    IsBot = p.IsBot,
                    TurnOrder = p.TurnOrder,
                    IsWinner = p.IsWinner
                });
            }

            string? winnerName = null;
            if (lobby.WinnerId.HasValue)
            {
                var winnerUser = await _dbContext.Users.FindAsync(lobby.WinnerId.Value);
                winnerName = winnerUser?.Name;
            }

            return new RussianRouletteStatusResponse
            {
                LobbyId = lobby.Id,
                LobbyCode = lobby.LobbyCode,
                Status = lobby.Status,
                CurrentPrizePool = lobby.CurrentPrizePool,
                TotalPlayers = players.Count,
                AlivePlayers = players.Count(p => p.IsAlive),
                Players = playerResponses,
                WinnerId = lobby.WinnerId,
                WinnerName = winnerName,
                SessionClosed = false 
            };
        }

        // =============================================nuevo añadido el 01/06/2026
        public async Task<List<RussianRouletteRound>> GetRoundHistory(Guid lobbyId)
        {
            // Verificar que el lobby existe
            var lobby = await _dbContext.RussianRouletteLobbies
                .FirstOrDefaultAsync(l => l.Id == lobbyId)
                ?? throw new KeyNotFoundException($"Lobby {lobbyId} not found");

            // Obtener rondas ordenadas
            return await _dbContext.RussianRouletteRounds
                .Where(r => r.LobbyId == lobbyId)
                .OrderBy(r => r.RoundNumber)
                .ToListAsync();
        }

    }
}
