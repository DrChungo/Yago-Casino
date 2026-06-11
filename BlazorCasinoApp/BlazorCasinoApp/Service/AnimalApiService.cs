using Chaos.BlazorCasinoApp.Auth;
using Chaos.Shared.Dto;
using Chaos.Shared.ResponseEntity;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Chaos.BlazorCasinoApp.Service
{
    public class AnimalApiService
    {
        private readonly HttpClient _http;
        private readonly JwtAuthenticationStateProvider _authProvider;
        private readonly IConfiguration _config;

        public AnimalApiService(
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

        // ── Obtener animales de un usuario ──
        public async Task<List<AnimalDto>> GetAnimalsByUserAsync(Guid userId)
        {
            await AttachTokenAsync();
            var baseUrl = _config["Api:UrlBase"];
            var endpoint = _config["Api:Endpoints:UrlBackendAPIGetAnimalsByUser"];
            var fullUrl = $"{baseUrl}{endpoint}/{userId}";

            var response = await _http.GetAsync(fullUrl);
            var raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return new List<AnimalDto>();

            var wrapper = System.Text.Json.JsonSerializer.Deserialize<AnimalListResponse>(raw,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return wrapper?.Data ?? new List<AnimalDto>();
        }

        // ── Crear y asignar animal a un usuario ──
        public async Task<bool> CreateAndAssignAnimalAsync(Guid userId)
        {
            await AttachTokenAsync();
            var baseUrl = _config["Api:UrlBase"];
            var url = _config["Api:Endpoints:UrlBackendAPICreateAndAssign"];
            var response = await _http.PostAsync($"{baseUrl}{url}/{userId}", null);

            Console.WriteLine($"[CreateAnimal] Status: {response.StatusCode}");
            var raw = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[CreateAnimal] Body: {raw}");

            return response.IsSuccessStatusCode;
        }

        // ── Eliminar animal por Id ──
        public async Task RemoveAnimalAsync(Guid animalId)
        {
            await AttachTokenAsync();
            var baseUrl = _config["Api:UrlBase"];
            var url = _config["Api:Endpoints:UrlBackendAPIRemoveAnimal"];
            var response = await _http.DeleteAsync($"{baseUrl}{url}/{animalId}");

            Console.WriteLine($"[RemoveAnimal] Status: {response.StatusCode}");
        }
    }
}