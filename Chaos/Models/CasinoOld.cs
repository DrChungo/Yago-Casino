namespace Chaos.Api.Models
{
    public class CasinoOld
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int GameCount { get; set; } = 4;
    }
}
