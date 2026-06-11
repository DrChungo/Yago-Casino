using System.Collections.Generic;

namespace Chaos.Api.Models
{
    public class Deck
    {
        public List<Card> Cards { get; set; } = new List<Card>();

        public int RemainingCards => Cards.Count;

        public Deck() { }

        public Deck(List<Card> cards)
        {
            Cards = cards;
        }
    }
}