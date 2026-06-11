using Chaos.Api.Models;

namespace Chaos.Api.Provider
{
    public class CasinoProvider
    {
        public CasinoOld Casino { get; }
        public AnimalShopOld AnimalShop { get; }

        public CasinoProvider()
        {
            var id = Guid.NewGuid();
            Casino = new CasinoOld
            {
                Id = id,
                Name = "Casino Royale",
                Location = "Las Vegas"

            };

            AnimalShop = new AnimalShopOld
            {
                CasinoId = id,
                Name = "Casino Royale shop"
            };
        }
    }
}