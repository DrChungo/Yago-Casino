using Chaos.Api.Interface;
using Chaos.Infraestructure.Models;

namespace Chaos.Api.Service
{
    public class SlotSessionService : ISlotSessionService
    {
        private readonly CasinoDBContext _dbContext;

        public SlotSessionService(CasinoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task  AddSession(SlotSession session)
        {
            await _dbContext.SlotSessions.AddAsync(session);
            await _dbContext.SaveChangesAsync();
        }
    }
}
