using Chaos.Api.RequestEntity;
using Chaos.Api.ResponseEntity;
using Microsoft.AspNetCore.Mvc;

namespace Chaos.Api.Interface
{
    public interface ILobbyService
    {
        Lobby CreateLobby(Guid userId);
        Lobby AddPlayerToLobby(string lobbyCode, Guid userId); 
        bool CheckUsersLobby(PlayerLobby playerKickLobby);
        Lobby GetLobbyById(Guid idLobby);
        Lobby KickPlayerLobby(PlayerLobby playerKickLobby, Guid requesterUserId);

        //nuevo añadido el 001/06/2026
        Lobby LeaveFromLobby(Guid lobbyId, Guid userId);
        List<PlayerInLobbyResponse> GetPlayersByLobby(Guid lobbyId);
        bool RemoveLobby(Guid lobby);
        List<Lobby> GetAllLobbies();
    }
}
