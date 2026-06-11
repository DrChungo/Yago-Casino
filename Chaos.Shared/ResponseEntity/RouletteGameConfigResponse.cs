using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Shared.ResponseEntity
{
    public class RouletteGameConfigResponse
    {
        public Guid Id { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string RouletteType { get; set; } = string.Empty;
        public bool HasZero { get; set; }
        public bool HasDoubleZero { get; set; }
        public int TotalNumbers { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public Guid GameId { get; set; }
        public string GameName { get; set; } = string.Empty;
    }
}

