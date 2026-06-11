using Chaos.Api.Interface;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service
{
    public class BlackjackSessionService : IBlackjackSessionService
    {
        private readonly IDbContextFactory<CasinoDBContext> _factory;

        public BlackjackSessionService(IDbContextFactory<CasinoDBContext> factory)
        {
            _factory = factory;
        }

        public async Task AddSession(BlackjackSession session)
        {
            using var context = _factory.CreateDbContext();

            await context.BlackjackSessions.AddAsync(session);
            await context.SaveChangesAsync();
        }
    }

}
