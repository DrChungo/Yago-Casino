using Chaos.Infraestructure.Models;

namespace Chaos.Api.Utils
{
    public class GenerateLobbyCode(CasinoDBContext casinoDBContext)
    {
        public readonly CasinoDBContext _dbContext = casinoDBContext;

        public static string GeneratorLobbyCode(CasinoDBContext dBContext)
        {
            string newCode;

            do
            {
               
                string randomPart = Guid.NewGuid().ToString("N")[..8].ToUpper();
                newCode = $"RR-{randomPart[..4]}-{randomPart[4..]}";
            }
            while (dBContext.RussianRouletteLobbies.Any(l => l.LobbyCode == newCode));

            return newCode;
        }
    }
}