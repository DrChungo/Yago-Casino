using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Shared.RequestEntity
{
    public class RouletteGameConfigRequest
    {
        public string TableName { get; set; } = string.Empty;
        public string RouletteType { get; set; } = string.Empty;
        public bool HasZero { get; set; }
        public bool HasDoubleZero { get; set; }
        public int TotalNumbers { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
