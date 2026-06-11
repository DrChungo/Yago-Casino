namespace Chaos.Api.RequestEntity.Config.AnimalValue
{
    public class CreateAnimalValueConfigRequest
    {
        public string AnimalType { get; set; }

        public int MinAge { get; set; }
        public int MaxAge { get; set; }

        public decimal MinWeight { get; set; }
        public decimal MaxWeight { get; set; }

        public decimal MinHeight { get; set; }
        public decimal MaxHeight { get; set; }

        public int MinHealth { get; set; }
        public int MaxHealth { get; set; }

        public string? ImageUrlNormal { get; set; }
        public string? ImageUrlMecha { get; set; }
        public string? Habitat { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        public Guid? UpdatedBy { get; set; }

    }
}
