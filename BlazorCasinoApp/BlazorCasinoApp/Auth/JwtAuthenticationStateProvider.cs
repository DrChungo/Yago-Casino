using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace Chaos.BlazorCasinoApp.Auth;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _js;
    private readonly NavigationManager _nav;           
    private Timer? _logoutTimer;                    
    private const string TokenKey = "authToken";

    public JwtAuthenticationStateProvider(IJSRuntime js, NavigationManager nav) 
    {
        _js = js;
        _nav = nav;
    }

    // ─────────────────────────────────────────
    // Al abrir la app verifica si el token expiró
    // ─────────────────────────────────────────
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
            return Anonymous();

        // ── NUEVO — verificar expiración ──
        var expiration = GetTokenExpiration(token);
        if (expiration is null || expiration <= DateTime.UtcNow)
        {
            // Token expirado → logout inmediato
            await MarkUserAsLoggedOut();
            return Anonymous();
        }

        // Token válido → programar auto-logout
        ScheduleAutoLogout(token);                   

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt", "name", ClaimTypes.Role);
        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }

    // ─────────────────────────────────────────
    // Login — guardar token y programar logout
    // ─────────────────────────────────────────
    public async Task MarkUserAsAuthenticated(string token)
    {
        await SetTokenAsync(token);
        ScheduleAutoLogout(token);                    

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt", "name", ClaimTypes.Role);
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(user))
        );
    }

    // ─────────────────────────────────────────
    // Logout — limpiar timer y redirigir
    // ─────────────────────────────────────────
    public async Task MarkUserAsLoggedOut()
    {
        _logoutTimer?.Dispose();                     
        _logoutTimer = null;                           

        await RemoveTokenAsync();

        NotifyAuthenticationStateChanged(
            Task.FromResult(Anonymous())
        );

        _nav.NavigateTo("/login", forceLoad: true);    
    }

    // ─────────────────────────────────────────
    // NUEVO — Leer expiración del JWT
    // ─────────────────────────────────────────
    private static DateTime? GetTokenExpiration(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.ValidTo; // UTC
        }
        catch { return null; }
    }

    // ─────────────────────────────────────────
    // Programar el timer con el tiempo restante
    // ─────────────────────────────────────────
    private void ScheduleAutoLogout(string token)
    {
        _logoutTimer?.Dispose();

        var expiration = GetTokenExpiration(token);
        if (expiration is null) return;

        var delay = expiration.Value - DateTime.UtcNow;
        if (delay <= TimeSpan.Zero)
        {
            _ = MarkUserAsLoggedOut();
            return;
        }

        _logoutTimer = new Timer(async _ =>
        {
            await MarkUserAsLoggedOut();
        }, null, delay, Timeout.InfiniteTimeSpan);
    }

    // ─────────────────────────────────────────
    // Token helpers — igual que antes
    // ─────────────────────────────────────────
    public async Task<string?> GetTokenAsync()
        => await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);

    private async Task SetTokenAsync(string token)
        => await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);

    private async Task RemoveTokenAsync()
        => await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);

    private static AuthenticationState Anonymous()
        => new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

    // ─────────────────────────────────────────
    // ParseClaimsFromJwt — igual que antes
    // ─────────────────────────────────────────
    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();

        var payload = jwt.Split('.')[1];
        payload = payload.Replace('-', '+').Replace('_', '/');
        switch (payload.Length % 4)
        {
            case 2: payload += "=="; break;
            case 3: payload += "="; break;
        }

        var jsonBytes = Convert.FromBase64String(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes);

        if (keyValuePairs is null) return claims;

        var roleKeys = new[] { "role", ClaimTypes.Role, "roles" };

        foreach (var roleKey in roleKeys)
        {
            if (keyValuePairs.TryGetValue(roleKey, out var roles))
            {
                if (roles.ValueKind == JsonValueKind.Array)
                    foreach (var role in roles.EnumerateArray())
                        claims.Add(new Claim(ClaimTypes.Role, role.GetString()!));
                else
                    claims.Add(new Claim(ClaimTypes.Role, roles.GetString()!));
                break;
            }
        }

        foreach (var kvp in keyValuePairs)
        {
            if (roleKeys.Contains(kvp.Key)) continue;

            var value = kvp.Value.ValueKind switch
            {
                JsonValueKind.String => kvp.Value.GetString() ?? "",
                JsonValueKind.Number => kvp.Value.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null => "",
                _ => kvp.Value.GetRawText()
            };

            claims.Add(new Claim(kvp.Key, value));
        }

        return claims;
    }
}