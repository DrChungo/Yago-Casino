using Chaos.Api.Interface;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service
{
    public class CoinFlipSessionService : ICoinFlipSessionService
    {
        private readonly IDbContextFactory<CasinoDBContext> _factory;

        public CoinFlipSessionService(IDbContextFactory<CasinoDBContext> factory)
        {
            _factory = factory;
        }

        public async Task AddSession(CoinFlipSession session)
        {
            using var context = _factory.CreateDbContext();
            await context.CoinFlipSessions.AddAsync(session);
            await context.SaveChangesAsync();
        }
    }
}
