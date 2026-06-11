using Chaos.Infraestructure.Models;

namespace Chaos.Api.Interface
{

    public interface ICoinFlipSessionService
    {
        Task AddSession(CoinFlipSession session);
    }

}
