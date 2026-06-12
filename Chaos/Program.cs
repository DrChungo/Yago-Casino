using Chaos.Api.Interface;
using Chaos.Api.Interface.Config;
using Chaos.Api.Middleware;
using Chaos.Api.Models;
using Chaos.Api.Provider;
using Chaos.Api.Service;
using Chaos.Api.Service.Config;
using Chaos.Infraestructure.Data;
using Chaos.Infraestructure.Extensions;
using Chaos.Infraestructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────
// Servicios base
// ─────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );
    });

// ─────────────────────────────────────────
// Swagger
// ─────────────────────────────────────────
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Chaos API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce el token JWT sin el prefijo Bearer."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// ─────────────────────────────────────────
//JWT Authentication
// ─────────────────────────────────────────
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

// ─────────────────────────────────────────
// Base de datos
// ─────────────────────────────────────────

builder.Services.AddDbContext<CasinoDBContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__efmigrationshistory", "casino")
    )
);

builder.Services.AddDbContextFactory<CasinoDBContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__efmigrationshistory", "casino")
    ),
    ServiceLifetime.Scoped
);


// ─────────────────────────────────────────
//CORS
// ─────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

// ─────────────────────────────────────────
//Servicios de la aplicación
// ─────────────────────────────────────────

builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddScoped<IAuthServiceBackOffice, AuthServiceBackOffice>();
builder.Services.AddScoped<IRankingService, RankingService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<ICasinoGamesService, CasinoGamesService>();
builder.Services.AddScoped<ICoinGameService, CoinGameService>();
builder.Services.AddScoped<ISlotPayoutRuleService, SlotPayoutRuleService>();
builder.Services.AddScoped<ISlotSymbolService, SlotSymbolService>();
builder.Services.AddScoped<ILobbyService, LobbyService>();
builder.Services.AddScoped<ICoinFlipSessionService, CoinFlipSessionService>();
builder.Services.AddScoped<IGameSessionervice, GameSessionervice>();
builder.Services.AddScoped<IActiveDrinkEffectService, ActiveDrinkEffectService>();
builder.Services.AddScoped<IBlackjackSessionService, BlackjackSessionService>();
builder.Services.AddScoped<IHigherLowerSessionService, HigherLowerSessionService>();
builder.Services.AddScoped<IHigherLowerGameService, HigherLowerGameService>();
builder.Services.AddScoped<IRouletteGameConfigService, RouletteGameConfigService>();

builder.Services.AddScoped<ICoinGameService, CoinGameService>();
builder.Services.AddScoped<ISlotGameConfigService, SlotGameConfigService>();
builder.Services.AddScoped<IAnimalValueConfigService, AnimalValueConfigService>();

builder.Services.AddScoped<ISlotGameConfigService, SlotGameConfigService>();
builder.Services.AddScoped<CasinoProvider>();

builder.Services.AddSingleton<Random>();
builder.Services.AddSingleton<IDeckService, DeckService>();
builder.Services.AddSingleton<ITokenBlackListService, TokenBlackListService>();

builder.Services.AddScoped<ISlotSessionService, SlotSessionService>();

//si va
builder.Services.AddScoped<IBlackjackConfigService, BlackjackConfigService>();

//backoffice
builder.Services.AddScoped<IRussianRouletteService, RussianRouletteService>();

//backend
builder.Services.AddScoped<IRussianRouletteServicee, RussianRouletteServicee>();


builder.Services.AddHttpContextAccessor();

// Infraestructura personalizada
builder.Services.AddInfrastructure(builder.Configuration);

// ─────────────────────────────────────────
// Build
// ─────────────────────────────────────────
var app = builder.Build();

// Inicialización de la base de datos
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CasinoDBContext>();
    CasinoDbInitializer.Initialize(context);
}

// ─────────────────────────────────────────
// Pipeline de middlewares
// ─────────────────────────────────────────

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TokenBlackListMiddleware>();

app.UseInfrastructure(); // movido antes de MapControllers
app.UseInfrastructure(); 

app.UseSwagger();
app.MapScalarApiReference(options =>
{
    options.OpenApiRoutePattern = "/swagger/v1/swagger.json";
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chaos API v1");
    c.RoutePrefix = "swagger";
});

app.MapScalarApiReference(options =>
{
    options.OpenApiRoutePattern = "/swagger/v1/swagger.json";
});

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();

// ─────────────────────────────────────────
//Endpoints mínimos
// ─────────────────────────────────────────
app.MapGet("/time/now", () => Results.Ok(new
{
    Status = "OK",
    Timestamp = DateTime.UtcNow
}));

app.MapGet("/status", () => Results.Ok(new StatusResponse
{
    ServiceName = "Chaos API",
    Environment = app.Environment.EnvironmentName,
    Version = "1.0.0",
    UpdatedAt = DateTime.UtcNow
}));

app.MapPost("/status", (StatusResponse input) =>
{
    input.UpdatedAt = DateTime.UtcNow;
    return Results.Ok(input);
});

app.Run();