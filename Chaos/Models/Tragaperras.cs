using Chaos.Infraestructure.Models;
using System.Reflection.PortableExecutable;

namespace Chaos.Api.Models
{
    public class Tragaperras
    {
        public decimal Reward { get; set; }
        public List<string[,]> ListMachines {  get; set; } = new();
        public List<string> AllRewards { get; set; } = new();
        public Game game { get; set; } = new Game();
        public SlotGameConfig Config { get; set; } = new();
        public List<SlotSymbol> SlotsSymbols { get; set; } = new();
        public SlotPayoutRule PayoutRule { get; set; } = new();
        public int CashBack {  get; set; }
    }
}
