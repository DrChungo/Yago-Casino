using Microsoft.EntityFrameworkCore;

namespace Chaos.Infraestructure.Models;

public partial class CasinoDBContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Casino");
    }
}