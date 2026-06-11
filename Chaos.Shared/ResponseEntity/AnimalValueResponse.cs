

namespace Chaos.Shared.ResponseEntity
{
    public class AnimalValueResponse
    {

        public Guid Id { get; set; }
        public string AnimalType { get; set; } = string.Empty;
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public decimal MinWeight { get; set; }
        public decimal MaxWeight { get; set; }
        public decimal MinHeight { get; set; }
        public decimal MaxHeight { get; set; }
        public int MinHealth { get; set; }
        public int MaxHealth { get; set; }
        public bool IsActive { get; set; }


        public string? ImageUrlNormal { get; set; } = string.Empty;
        public string? ImageUrlMecha { get; set; } = string.Empty;
        public string? Habitat { get; set; } = string.Empty;

        public Guid? UpdatedBy { get; set; }
    }
}
