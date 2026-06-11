namespace Chaos.Api.ResponseEntity.Config.AnimalValue
{
    public class AnimalImage
    {
        public Guid Id { get; set; }
        public string AnimalType { get; set; } = string.Empty;
        public string? ImageUrlNormal { get; set; }
        public string? ImageUrlMecha { get; set; }
        public string? Habitat { get; set; } = string.Empty;
    }
}
