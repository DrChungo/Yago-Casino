using Chaos.Infraestructure.Models;

namespace Chaos.Api.Interface
{

    public interface IGameSessionervice
    {
        Task AddGameSession(GameSession session);
    }

}
