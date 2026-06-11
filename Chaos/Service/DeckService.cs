using Chaos.Api.Enums;
using Chaos.Api.Interface;
using Chaos.Api.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Chaos.Api.Service
{
    public class DeckService : IDeckService
    {
        private List<Card> _cards;
        private Random _random = Random.Shared; 

        public int RemainingCards => _cards.Count;

        public DeckService(Random random)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _cards = new List<Card>();
            InitializeDeck();
        }

        // Crear 52 cartas mediante 2 foreach
        public void InitializeDeck()
        {
            _cards.Clear();

            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
                {
                    _cards.Add(new Card(rank, suit));
                }
            }
        }

        public void Shuffle()
        {
            _random.Shuffle(CollectionsMarshal.AsSpan(_cards));
        }

        public Card CatchCard()
        {
            if (_cards.Count == 0)
            {
                throw new InvalidOperationException("No cards left in the deck");
            }

            Card card = _cards[0];
            _cards.RemoveAt(0);
            return card;
        }

        public void Reset()
        {
            InitializeDeck();
            Shuffle();
        }


        public void LoadFrom(List<Card> cards)
        {
            _cards = cards;
        }


        public List<Card> GetRemainingCards()
        {
            return new List<Card>(_cards);
        }


    }
}