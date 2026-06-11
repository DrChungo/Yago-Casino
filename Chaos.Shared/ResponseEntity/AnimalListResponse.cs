using Chaos.Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Shared.ResponseEntity
{

    public class AnimalListResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        // ✅ Lista vacía por defecto, nunca null
        public List<AnimalDto> Data { get; set; } = new List<AnimalDto>();

    }
}
