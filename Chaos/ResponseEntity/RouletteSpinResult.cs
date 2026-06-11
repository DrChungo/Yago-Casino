namespace Chaos.Api.ResponseEntity
{
    public class RouletteSpinResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<RouletteAnimalDto> Candidates { get; set; } = [];
        public RouletteAnimalDto? Winner { get; set; }                   
    }
}
