using Chaos.Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Shared.ResponseEntity
{
    public class AnimalResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public AnimalDto? Data { get; set; }

    }
}
