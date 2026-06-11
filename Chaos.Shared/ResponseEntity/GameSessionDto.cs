using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Shared.ResponseEntity
{
    public class GameSessionDto
    {
        public Guid Id { get; set; }
        public string GameName { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string? EndDate { get; set; }      
        public decimal MoneyDelta { get; set; }
        public bool IsWin { get; set; }
        public int RoundsPlayed { get; set; }   
    }
}
