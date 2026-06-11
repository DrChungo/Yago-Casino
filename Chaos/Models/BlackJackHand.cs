using Chaos.Api.Enums;

namespace Chaos.Api.Models
{
    public class BlackJackHand
    {
        public List<Card> Cards { get; set; }
        public int Score { get; private set; }
        public bool IsBusted { get; private set; }
        public bool IsTwentyOne { get; private set; }

        public BlackJackHand()
        {
            Cards = new List<Card>();
            Score = 0;
            IsBusted = false;
            IsTwentyOne = false;
        }

        public void AddCard(Card card)
        {
            Cards.Add(card);
            CalculateScore();
        }

        private void CalculateScore()
        {
            int score = 0;
            int aceCount = 0;

            // Sumar valores de todas las cartas
            foreach (var card in Cards)
            {
                if (card.Rank == CardRank.Ace)
                {
                    aceCount++;
                    score += 11; // Inicialmente contar Ace como 11
                }
                else
                {
                    score += card.BlackjackValue;
                }
            }

            // Ajustar Aces si es necesario de 11 a 1
            while (score > 21 && aceCount > 0)
            {
                score -= 10; 
                aceCount--;
            }

            Score = score;
            IsBusted = score > 21;
            IsTwentyOne = score == 21;
        }

        public string GetHandDisplay()
        {
            return string.Join(", ", Cards.Select(c => c.DisplayName));
        }
    }
}

