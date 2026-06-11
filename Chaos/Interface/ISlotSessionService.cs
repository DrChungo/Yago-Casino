using Chaos.Infraestructure.Models;

namespace Chaos.Api.Interface
{
    public interface ISlotSessionService
    {
        Task AddSession(SlotSession session);
    }
}
