using Chaos.Api.Enums;

namespace Chaos.Api.Models
{
    public class Card(CardRank rank, CardSuit suit)
    {
        public CardRank Rank { get; set; } = rank;
        public CardSuit Suit { get; set; } = suit;
        public int Value { get; set; } = (int)rank;

        //Valor para Blackjack
        public int BlackjackValue => Rank switch
        {
            CardRank.Jack => 10,
            CardRank.Queen => 10,
            CardRank.King => 10,
            CardRank.Ace => 11,
            _ => (int)Rank
        };
        public string DisplayName { get; set; } = $"{rank} of {suit}";

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
