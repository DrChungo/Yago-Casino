using Chaos.Api.Models;
using Chaos.Infraestructure.Models;

namespace Chaos.Api.Interface
{

    public interface IHigherLowerSessionService
    {
        Task AddSession(HigherLowerSession session);
    }

}
