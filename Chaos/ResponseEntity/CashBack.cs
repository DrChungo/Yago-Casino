namespace Chaos.Api.ResponseEntity
{
    public class CashBack
    {
        public int Bet {  get; set; }
        public int MoneyBack { get; set; }
        public int PositionEuropeanRoulette { get; set; }

        public string ResultMessage { get; set; } = string.Empty;

    }
}
