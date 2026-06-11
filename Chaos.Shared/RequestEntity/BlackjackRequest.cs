using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Shared.RequestEntity
{
    public class BlackjackRequest
    {
        public string TableName { get; set; } = string.Empty;
        public int NumberOfDecks { get; set; } = 6;
        public int MaxPlayers { get; set; } = 6;
        public int DealerStandsOn { get; set; } = 17;
        public decimal BlackjackPayout { get; set; } = 1.50m;
        public bool AllowDoubleDown { get; set; } = true;
        public bool AllowInsurance { get; set; } = true;
        public bool IsActive { get; set; } = true;
    }
}
