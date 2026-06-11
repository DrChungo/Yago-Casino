using Chaos.Shared.ResponseEntity;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using Chaos.BlazorCasinoApp.Auth;
using System.Net.Http.Json;
using Chaos.Shared.Dto;

namespace Chaos.BlazorCasinoApp.Service
{
    public class UserManagementService
    {
        private readonly HttpClient _http;
        private readonly JwtAuthenticationStateProvider _authProvider;
        private readonly IConfiguration _config;

        public UserManagementService(
            HttpClient http,
            AuthenticationStateProvider authProvider,
            IConfiguration config)
        {
            _http = http;
            _authProvider = (JwtAuthenticationStateProvider)authProvider;
            _config = config;
        }

        private async Task AttachTokenAsync()
        {
            var token = await _authProvider.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        // ── Obtener todos los usuarios ──
        public async Task<List<UserConfigDto>> GetAllUsersAsync()
        {
            await AttachTokenAsync();

            var token = await _authProvider.GetTokenAsync();
            Console.WriteLine($"TOKEN ENVIADO: {token}");
            Console.WriteLine($"AUTH HEADER: {_http.DefaultRequestHeaders.Authorization}");

            var url = _config["Api:Endpoints:UrlBackendAPIGetAllUsers"];
            Console.WriteLine($"URL: {_http.BaseAddress}{url}");

            return await _http.GetFromJsonAsync<List<UserConfigDto>>(url)
                   ?? new List<UserConfigDto>();
        }

        // ── Obtener sesiones de un usuario ──
        public async Task<List<GameSessionDto>> GetUserSessionsAsync(Guid userId)
        {
            await AttachTokenAsync();
            var baseUrl = _config["Api:Endpoints:UrlBackendAPIGetUserSessions"];
            return await _http.GetFromJsonAsync<List<GameSessionDto>>(
                           $"{baseUrl}/{userId}/sessions")
                   ?? new List<GameSessionDto>();
        }

        // ── Promover a Admin ──
        public async Task MakeAdminAsync(Guid userId)
        {
            await AttachTokenAsync();
            var url = _config["Api:Endpoints:UrlBackendAPIAuthMakeAdmin"];
            await _http.PatchAsync($"{url}/{userId}", null);
        }

        // ── Revocar Admin ──
        public async Task RevokeAdminAsync(Guid userId)
        {
            await AttachTokenAsync();
            var url = _config["Api:Endpoints:UrlBackendAPIAuthRevokeAdmin"];
            await _http.PatchAsync($"{url}/{userId}", null);
        }

        // ── Desactivar usuario ──
        public async Task DeactivateUserAsync(Guid userId)
        {
            await AttachTokenAsync();
            var url = _config["Api:Endpoints:UrlBackendAPIDeactivateUser"];
            await _http.PatchAsync($"{url}/{userId}/deactivate", null);
        }

        // ── Activar usuario ──
        public async Task ActivateUserAsync(Guid userId)
        {
            await AttachTokenAsync();
            var url = _config["Api:Endpoints:UrlBackendAPIDeactivateUser"];
            await _http.PatchAsync($"{url}/{userId}/activate", null);
        }

        // ── Añadir balance ──
        public async Task AddBalanceAsync(Guid userId, decimal amount)
        {
            await AttachTokenAsync();

            var amountInt = (int)amount;
            var fullUrl = $"User/{userId}/balance?amount={amountInt}";

            Console.WriteLine($"[AddBalance] URL final: {_http.BaseAddress}{fullUrl}");

            var response = await _http.PatchAsync(fullUrl, null);
            Console.WriteLine($"[AddBalance] Status: {response.StatusCode}");

            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[AddBalance] Body: {body}");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error: {response.StatusCode} - {body}");
        }
    }
}