using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Shared.Dto
{
    public class AnimalDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TypeAnimal { get; set; } = string.Empty;
        public int Age { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        public string Health { get; set; } = string.Empty;
        public int Value { get; set; }
        public Guid? OwnerId { get; set; }
        public bool IsAvailable { get; set; }
        public bool? Rarity { get; set; }

    }
}
