namespace Chaos.Api.RequestEntity.Config.Roulette
{
    public class CreateRouletteGameConfigRequest
    {
        public string TableName { get; set; }
        public string RouletteType { get; set; }
        public bool HasZero { get; set; }
        public bool HasDoubleZero { get; set; }
        public int TotalNumbers { get; set; }
        public bool IsActive { get; set; }
    }
}
