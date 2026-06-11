using Chaos.Shared.RequestEntity;
using Chaos.Shared.ResponseEntity;
using Chaos.BlazorCasinoApp.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Chaos.BlazorCasinoApp.Service
{

    public class AuthBackOfficeService
    {
        private readonly HttpClient _http;
        private readonly JwtAuthenticationStateProvider _authProvider;

        public AuthBackOfficeService(HttpClient http, AuthenticationStateProvider authProvider)
        {
            _http = http;
            _authProvider = (JwtAuthenticationStateProvider)authProvider;
        }

        // ─────────────────────────────────────────
        // Adjunta el token JWT al header de la petición
        // ─────────────────────────────────────────
        private async Task AttachTokenAsync()
        {
            var token = await _authProvider.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        // ─────────────────────────────────────────
        // Register
        // ─────────────────────────────────────────
        public async Task<AuthBackOfficeResponse?> RegisterAsync(RegisterBackOfficeRequest request)
        {
            var response = await _http.PostAsJsonAsync("backoffice/auth/register", request);
            return await response.Content.ReadFromJsonAsync<AuthBackOfficeResponse>();
        }

        // ─────────────────────────────────────────
        // Login
        // ─────────────────────────────────────────
        public async Task<AuthBackOfficeResponse?> LoginAsync(LoginBackOfficeRequest request)
        {
            var response = await _http.PostAsJsonAsync("backoffice/auth/login", request);
            var result = await response.Content.ReadFromJsonAsync<AuthBackOfficeResponse>();

            if (result?.Token is not null)
                await _authProvider.MarkUserAsAuthenticated(result.Token);

            return result;
        }

        // ─────────────────────────────────────────
        // Logout
        // ─────────────────────────────────────────
        public async Task LogoutAsync()
        {
            await _authProvider.MarkUserAsLoggedOut();
            _http.DefaultRequestHeaders.Authorization = null;
        }

        // ─────────────────────────────────────────
        // Make Admin
        // ─────────────────────────────────────────
        public async Task<AuthBackOfficeResponse?> MakeAdminAsync(Guid userId)
        {
            await AttachTokenAsync();
            var response = await _http.PatchAsync(
                $"backoffice/auth/make-admin/{userId}", null);
            return await response.Content.ReadFromJsonAsync<AuthBackOfficeResponse>();
        }

        // ─────────────────────────────────────────
        // Revoke Admin
        // ─────────────────────────────────────────
        public async Task<AuthBackOfficeResponse?> RevokeAdminAsync(Guid userId)
        {
            await AttachTokenAsync();
            var response = await _http.PatchAsync(
                $"backoffice/auth/revoke/{userId}", null);
            return await response.Content.ReadFromJsonAsync<AuthBackOfficeResponse>();
        }
    }
}