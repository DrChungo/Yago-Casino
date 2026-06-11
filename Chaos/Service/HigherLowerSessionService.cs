using Chaos.Api.Interface;
using Chaos.Api.Models;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service
{
    public class HigherLowerSessionService : IHigherLowerSessionService
    {
        private readonly IDbContextFactory<CasinoDBContext> _factory;

        public HigherLowerSessionService(IDbContextFactory<CasinoDBContext> factory)
        {
            _factory = factory;
        }

        public async Task AddSession(HigherLowerSession session)
        {
            using var context = _factory.CreateDbContext();

            await context.HigherLowerSessions.AddAsync(session);
            await context.SaveChangesAsync();
        }
    }

}
