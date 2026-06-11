namespace Chaos.Api.ResponseEntity
{
    public class UserWalletRankingResponse
    {
        public string Name { get; set; } = string.Empty;
        public decimal Wallet { get; set; }
        public string Mail { get; set; } = string.Empty;
    }

    public class UserTotalAnimalValueResponse
    {
        public string Name { get; set; } = string.Empty;
        public decimal TotalAnimalValue { get; set; }
        public string Mail { get; set; } = string.Empty;

    }
}
