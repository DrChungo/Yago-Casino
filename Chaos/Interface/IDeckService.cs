using Chaos.Api.Models;
using Chaos.Api.RequestEntity;
using Chaos.Api.ResponseEntity;

namespace Chaos.Api.Interface
{
    public interface IDeckService
    {

       public void InitializeDeck();
       public void Shuffle();
       public Card CatchCard();
       public void Reset();
        public int RemainingCards { get; }

        public void LoadFrom(List<Card> cards);

        public List<Card> GetRemainingCards();

    }
}
