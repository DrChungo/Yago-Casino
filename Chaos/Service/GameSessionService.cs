using Chaos.Api.Interface;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service
{
    public class GameSessionervice : IGameSessionervice
    {
        private readonly IDbContextFactory<CasinoDBContext> _factory;

        public GameSessionervice(IDbContextFactory<CasinoDBContext> factory)
        {
            _factory = factory;
        }

        public async Task AddGameSession(GameSession session)
        {
            using var context = _factory.CreateDbContext();
            await context.GameSessions.AddAsync(session);
            await context.SaveChangesAsync();
        }
    }
}
