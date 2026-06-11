using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Chaos.Infraestructure.Models;
using Chaos.Infraestructure.Data;
using Microsoft.AspNetCore.Builder;

namespace Chaos.Infraestructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CasinoDBContext>();


            CasinoDbInitializer.Initialize(context);

            return app;
        }
    }
}