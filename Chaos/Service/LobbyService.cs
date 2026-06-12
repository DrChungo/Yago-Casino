using Chaos.Api.Interface;
using Chaos.Api.Models;
using Chaos.Api.RequestEntity;
using Chaos.Api.ResponseEntity;
using Chaos.Api.Utils;
using Chaos.Infraestructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service
{
    public class LobbyService : ILobbyService
    {
        public readonly CasinoDBContext _casinoDBContext;
        public LobbyService(CasinoDBContext casinoDB) => _casinoDBContext = casinoDB;

        public Lobby AddPlayerToLobby(string lobbyCode, Guid userId)
        {
            bool userExists = _casinoDBContext.Users.Any(u => u.Id == userId);
            if (!userExists)
                throw new Exception($"Usuario con Id {userId} no existe en la base de datos.");

            RussianRouletteLobby lobby = _casinoDBContext.RussianRouletteLobbies
                .Include(l => l.RussianRouletteGameConfig)
                .Include(l => l.RussianRoulettePlayers)
                .FirstOrDefault(l => l.LobbyCode == lobbyCode);

            if (lobby == null)
                throw new Exception($"Lobby con código {lobbyCode} no existe.");

            if (lobby.Status != "Waiting")
                throw new Exception($"El lobby ya está en estado '{lobby.Status}'. No se puede unir.");

            bool alreadyIn = lobby.RussianRoulettePlayers.Any(p => p.UserId == userId);
            if (alreadyIn)
                throw new Exception("El usuario ya está en este lobby.");

            int maxPlayers = lobby.RussianRouletteGameConfig?.MaxPlayers ?? 6;
            if (lobby.RussianRoulettePlayers.Count >= maxPlayers)
                throw new Exception($"El lobby está lleno. Máximo {maxPlayers} jugadores.");

            RussianRoulettePlayer newPlayer = new()
            {
                Id = Guid.NewGuid(),
                IsBot = false,
                BotName = null,
                TurnOrder = lobby.RussianRoulettePlayers.Count + 1,
                IsAlive = true,
                IsWinner = false,
                EliminatedAt = null,
                JoinedAt = DateTime.UtcNow.ToString(),
                UserId = userId,
                LobbyId = lobby.Id
            };

            _casinoDBContext.Add(newPlayer);
            _casinoDBContext.SaveChanges();


            var updatedPlayers = _casinoDBContext.RussianRoulettePlayers
                .Include(p => p.User)
                .Where(p => p.LobbyId == lobby.Id)
                .OrderBy(p => p.JoinedAt)
                .ToList();

            var master = updatedPlayers.FirstOrDefault();

            return new Lobby()
            {
                IdLobby = lobby.Id,
                MasterOfLobby = master?.UserId ?? userId,
                NameOfMaster = master?.User?.Name ?? "Unknown",
                NameOfPlayers = updatedPlayers.Select(p => p.User?.Name ?? "Unknown").ToList(),
                Players = updatedPlayers.Select(p => p.Id).ToList(),
                LobbyCode = lobby.LobbyCode,
                StartedAt = lobby.StartedAt ?? string.Empty,
                Status = lobby.Status ?? string.Empty
            };
        }


        public Lobby CreateLobby(Guid userId)
        {
            // Verificar que el usuario existe en BD
            bool userExists = _casinoDBContext.Users.Any(u => u.Id == userId);
            if (!userExists)
                throw new Exception($"Usuario con Id {userId} no existe en la base de datos.");

            // ✅ Cargar el usuario directamente para usarlo después
            var currentUser = _casinoDBContext.Users.FirstOrDefault(u => u.Id == userId);

            // Verificar que el usuario no está ya en otro lobby activo
            bool alreadyInLobby = _casinoDBContext.RussianRoulettePlayers
                .Any(p => p.UserId == userId && p.Lobby.Status == "Waiting");

            var player = _casinoDBContext.RussianRoulettePlayers
                .FirstOrDefault(p => p.UserId == userId);

            if (alreadyInLobby)
            {
                var lobby = _casinoDBContext.RussianRouletteLobbies
                    .FirstOrDefault(l => l.Id == player.LobbyId);

                List<RussianRoulettePlayer> playersInLobby = _casinoDBContext.RussianRoulettePlayers
                    .Where(p => p.LobbyId == lobby.Id)
                    .ToList();

                RussianRoulettePlayer master = playersInLobby
                    .OrderBy(p => p.JoinedAt)
                    .FirstOrDefault();

                var playerUserIds = playersInLobby.Select(p => p.UserId).ToList();

                var playerNames = _casinoDBContext.Users
                    .Where(u => playerUserIds.Contains(u.Id))
                    .Select(u => u.Name)
                    .ToList();

                return new Lobby()
                {
                    IdLobby = lobby.Id,
                    MasterOfLobby = master.Id,
                    NameOfMaster = currentUser?.Name ?? "Unknown",
                    LobbyCode = lobby.LobbyCode,
                    NameOfPlayers = playerNames,
                    Players = playersInLobby.Select(p => p.Id).ToList(),
                    StartedAt = lobby.StartedAt ?? string.Empty,
                    Status = lobby.Status ?? string.Empty
                };
            }

            string generatedCode = GenerateLobbyCode.GeneratorLobbyCode(_casinoDBContext);
            Console.WriteLine($"[CreateLobby] Code generated: {generatedCode}");

            RussianRouletteGameConfig config = _casinoDBContext
                .RussianRouletteGameConfigs
                .FirstOrDefault();

            RussianRouletteLobby lobbyDataBase = new()
            {
                Id = Guid.NewGuid(),
                LobbyCode = generatedCode,
                Status = "Waiting",
                CurrentPrizePool = config?.FixedPrizePool ?? 500,
                WinnerId = null,
                CreatedAt = DateTime.UtcNow.ToString(),
                StartedAt = null,
                FinishedAt = null,
                RussianRouletteGameConfig = config
            };

            _casinoDBContext.Add(lobbyDataBase);

            RussianRoulettePlayer masterPlayer = new()
            {
                Id = Guid.NewGuid(),
                IsBot = false,
                BotName = null,
                TurnOrder = 1,
                IsAlive = true,
                IsWinner = false,
                EliminatedAt = null,
                JoinedAt = DateTime.UtcNow.ToString(),
                UserId = userId,
                LobbyId = lobbyDataBase.Id,
            };

            _casinoDBContext.Add(masterPlayer);
            _casinoDBContext.SaveChanges();

            return new Lobby()
            {
                IdLobby = lobbyDataBase.Id,
                MasterOfLobby = userId,
                NameOfMaster = currentUser?.Name ?? "Unknown",
                Players = new List<Guid> { masterPlayer.Id },
                LobbyCode = lobbyDataBase.LobbyCode,
                NameOfPlayers = new List<string> { currentUser?.Name ?? "Unknown" },
                StartedAt = lobbyDataBase.StartedAt ?? string.Empty,
                Status = lobbyDataBase.Status ?? string.Empty
            };
        }

        public bool CheckUsersLobby(PlayerLobby playerKickLobby)
        {
            List<RussianRoulettePlayer> playersFromLoby = _casinoDBContext.RussianRoulettePlayers.Where(p => p.LobbyId == playerKickLobby.IdLobby).ToList();

            if (playersFromLoby.Any(p => p.Id == playerKickLobby.IdPlayer)) return true;
            return false;
        }


        public Lobby GetLobbyById(Guid idLobby)
        {
            var lobby = _casinoDBContext.RussianRouletteLobbies
                .FirstOrDefault(L => L.Id == idLobby);

            if (lobby == null) return null;

            List<RussianRoulettePlayer> playersInLobby = _casinoDBContext.RussianRoulettePlayers
                .Include(p => p.User) 
                .Where(p => p.LobbyId == idLobby)
                .ToList();

            RussianRoulettePlayer master = playersInLobby
                .OrderBy(player => player.JoinedAt)
                .FirstOrDefault();

            if (master == null) return null;

            return new Lobby()
            {
                IdLobby = lobby.Id,
                MasterOfLobby = master.Id,
                NameOfMaster = master?.User.Name ?? "Unknown",
                Players = playersInLobby.Select(player => player.Id).ToList(),
                //En Waiting solo hay humanos → solo necesitas User?.Name
                NameOfPlayers = playersInLobby.Where(p => !p.IsBot).Select(p => p.User?.Name ?? "Unknown").ToList(),
                LobbyCode = lobby.LobbyCode,
                StartedAt = lobby.StartedAt ?? string.Empty,
                Status = lobby.Status ?? string.Empty
            };
        }

        public Lobby KickPlayerLobby(PlayerLobby playerKickLobby)
        {
            RussianRouletteLobby lobby = _casinoDBContext.RussianRouletteLobbies
                .FirstOrDefault(L => L.Id == playerKickLobby.IdLobby);

            if (lobby == null)
                throw new InvalidOperationException("Lobby not found.");

            List<RussianRoulettePlayer> playersInLobby = _casinoDBContext.RussianRoulettePlayers
                .Include(p => p.User)                         
                .Where(p => p.LobbyId == playerKickLobby.IdLobby)
                .OrderBy(p => p.JoinedAt)
                .ToList();

            RussianRoulettePlayer master = playersInLobby.FirstOrDefault();

            RussianRoulettePlayer playerToRemove = playersInLobby
                .FirstOrDefault(p => p.Id == playerKickLobby.IdPlayer);

            if (playerToRemove != null && playerToRemove.Id != master?.Id)
            {
                _casinoDBContext.RussianRoulettePlayers.Remove(playerToRemove);
                _casinoDBContext.SaveChanges();

                playersInLobby.Remove(playerToRemove);
            }

            return new()
            {
                MasterOfLobby = master?.Id ?? Guid.Empty,
                Players = playersInLobby.Select(p => p.Id).ToList(),
                NameOfPlayers = playersInLobby.Select(p => p.User.Name).ToList(),
                LobbyCode = lobby.LobbyCode,
                NameOfMaster = master?.User?.Name ?? "Unknown",      
                StartedAt = lobby.StartedAt ?? string.Empty,
                Status = lobby.Status ?? string.Empty
            };
        }


        //NUEVO AÑADIDO 01/06/26
        public List<PlayerInLobbyResponse> GetPlayersByLobby(Guid lobbyId)
        {
            // Verificar que el lobby existe
            var lobby = _casinoDBContext.RussianRouletteLobbies
                .FirstOrDefault(l => l.Id == lobbyId)
                ?? throw new Exception($"Lobby con Id {lobbyId} no existe.");

            // Obtener jugadores del lobby
            var players = _casinoDBContext.RussianRoulettePlayers
                .Where(p => p.LobbyId == lobbyId)
                .OrderBy(p => p.TurnOrder)
                .ToList();

            // Determinar el master (el primero en unirse)
            var masterId = players.OrderBy(p => p.JoinedAt).FirstOrDefault()?.UserId;

            return players.Select(p => new PlayerInLobbyResponse
            {
                PlayerId = p.Id,
                UserId = p.UserId,
                IsBot = p.IsBot,
                BotName = p.BotName,
                IsAlive = p.IsAlive,
                IsWinner = p.IsWinner,
                TurnOrder = p.TurnOrder,
                JoinedAt = p.JoinedAt,
                IsMaster = p.UserId == masterId
            }).ToList();
        }

        public Lobby LeaveFromLobby(Guid lobbyId, Guid userId)
        {
            // Verificar que el lobby existe
            var lobby = _casinoDBContext.RussianRouletteLobbies
                .Include(l => l.RussianRoulettePlayers)
                .FirstOrDefault(l => l.Id == lobbyId)
                ?? throw new Exception($"Lobby con Id {lobbyId} no existe.");

            // Solo se puede salir si el lobby está en Waiting
            if (lobby.Status != "Waiting")
                throw new Exception($"No puedes salir de un lobby en estado '{lobby.Status}'.");

            // Verificar que el jugador está en el lobby
            var player = lobby.RussianRoulettePlayers
                .FirstOrDefault(p => p.UserId == userId)
                ?? throw new Exception("El usuario no está en este lobby.");

            // Eliminar al jugador
            _casinoDBContext.RussianRoulettePlayers.Remove(player);
            _casinoDBContext.SaveChanges();

            // Recargar jugadores restantes
            var remainingPlayers = _casinoDBContext.RussianRoulettePlayers
                .Where(p => p.LobbyId == lobbyId)
                .OrderBy(p => p.JoinedAt)
                .ToList();

            // Si no quedan jugadores → eliminar el lobby
            if (!remainingPlayers.Any())
            {
                _casinoDBContext.RussianRouletteLobbies.Remove(lobby);
                _casinoDBContext.SaveChanges();

                return new Lobby
                {
                    LobbyCode = lobby.LobbyCode,
                    MasterOfLobby = Guid.Empty,
                    Players = new List<Guid>()
                };
            }

            var master = remainingPlayers.First();

            return new Lobby
            {
                MasterOfLobby = master.UserId ?? Guid.Empty,
                Players = remainingPlayers.Select(p => p.Id).ToList(),
                LobbyCode = lobby.LobbyCode,
                StartedAt = lobby.StartedAt ?? string.Empty,
                Status = lobby.Status ?? string.Empty
            };
        }

        public bool RemoveLobby(Guid lobbyId)
        {
            var lobby = _casinoDBContext.RussianRouletteLobbies
                .FirstOrDefault(l => l.Id == lobbyId);

            if (lobby == null) return false;

            _casinoDBContext.RussianRouletteLobbies.Remove(lobby);
            _casinoDBContext.SaveChanges();

            return true;
        }

        public List<Lobby> GetAllLobbies()
        {
            var AllLobbiesDatabase = _casinoDBContext.RussianRouletteLobbies.Where(lobby => lobby.Status == "Waiting").ToList();

            List<Lobby> getLobbies = new();
            foreach (var lobby in AllLobbiesDatabase)
            {
                var player = _casinoDBContext.RussianRoulettePlayers.FirstOrDefault(master => master.LobbyId == lobby.Id);
                List<Guid> players = _casinoDBContext.RussianRoulettePlayers.Where(player => player.LobbyId == lobby.Id).Select(player => player.Id).ToList();
                var master = _casinoDBContext.RussianRoulettePlayers.Where(players => players.LobbyId == lobby.Id).OrderBy(user => user.JoinedAt).FirstOrDefault();
                
                List<string> nameOfPlayers = _casinoDBContext.Users.Where(user => players.Contains(user.Id)).Select(user => user.Name).ToList();
                string nameMaster = _casinoDBContext.Users.FirstOrDefault(nameMaster => nameMaster.Id == master.UserId).Name ?? "Unknow";

                var addLobby = new Lobby
                {
                    IdLobby = lobby.Id,
                    MasterOfLobby = master.Id,
                    NameOfMaster = nameMaster,
                    Players = players,
                    NameOfPlayers = nameOfPlayers,
                    LobbyCode = lobby.LobbyCode,
                    StartedAt = lobby.StartedAt ?? string.Empty,
                    Status = lobby.Status ?? string.Empty
                };
                getLobbies.Add(addLobby);
            }
            return getLobbies;
        }
    }
}