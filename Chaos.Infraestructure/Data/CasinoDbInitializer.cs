using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Infraestructure.Data
{
public static class CasinoDbInitializer
    {
        public static void Initialize(CasinoDBContext context)
        {
            //context.Database.Migrate();
            context.Database.CanConnect();

            // ── 1. USERS ─────────────────────────────────────────────────────────
        }
    }
}
