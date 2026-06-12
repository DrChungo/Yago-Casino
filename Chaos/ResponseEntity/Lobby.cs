using Chaos.Api.Utils;
using Microsoft.AspNetCore.Components.Forms;
using System.CodeDom.Compiler;

namespace Chaos.Api.ResponseEntity
{
    public class Lobby
    {
        public Guid IdLobby { get; set; }
        public Guid MasterOfLobby { get; set; }
        public string NameOfMaster { get; set; }
        public List<Guid> Players { get; set; } = new();
        public List<string> NameOfPlayers { get; set; } = new();
        public string LobbyCode { get; set; } = string.Empty;

        public string StartedAt { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
