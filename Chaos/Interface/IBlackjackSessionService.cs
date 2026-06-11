using Chaos.Infraestructure.Models;

namespace Chaos.Api.Interface
{

    public interface IBlackjackSessionService
    {
        Task AddSession(BlackjackSession session);
    }
}
