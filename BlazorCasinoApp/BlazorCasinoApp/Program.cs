using Chaos.BlazorCasinoApp;
using Chaos.BlazorCasinoApp.Auth;
using Chaos.BlazorCasinoApp.IApiService;
using Chaos.BlazorCasinoApp.IApiService.Slots;
using Chaos.BlazorCasinoApp.Service;
using Chaos.BlazorCasinoApp.Service.Slots;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["Api:UrlBase"]
    ?? throw new InvalidOperationException("Api:UrlBase no está configurado.");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthenticationStateProvider>());

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthBackOfficeService>();
builder.Services.AddScoped<UserManagementService>();



builder.Services.AddScoped<ICoinGameService, CoinGameApiService>();
builder.Services.AddScoped<ISlotGameService, SlotGameConfigApiService>();

builder.Services.AddScoped<ISlotSymbolService, SlotSymbolApiService>();
builder.Services.AddScoped<ISlotPayoutRuleService, SlotPayoutRuleApiService>();
builder.Services.AddScoped<IAnimalValueConfigService, AnimalValueConfigApiService>();
builder.Services.AddScoped<IBlackjackConfigService, BlackjackConfigApiService>();
builder.Services.AddScoped<IHigherLowerGameService, HigherLowerGameApiService>();
builder.Services.AddScoped<IRussianRouletteService, RussianRouletteApiService>();
builder.Services.AddScoped<AnimalApiService>();
builder.Services.AddScoped<IRouletteGameConfigService, RouletteGameConfigApiService>();

await builder.Build().RunAsync();
