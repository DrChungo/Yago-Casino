using Chaos.BlazorCasinoApp.IApiService;
using Chaos.BlazorCasinoApp.IApiService.Slots;
using Chaos.Shared.RequestEntity.SlotGame;

using Chaos.Shared.ResponseEntity.SlotGame;
using System.Net.Http.Json;

namespace Chaos.BlazorCasinoApp.Service.Slots
{
    public class SlotSymbolApiService : ISlotSymbolService
    {
        private readonly HttpClient _http;
        private readonly string _url;

        public SlotSymbolApiService(HttpClient http, IConfiguration configuration)
        {
            _http = http;

            string baseUrl = configuration["Api:UrlBase"]
                ?? throw new Exception("No se encontró Api:UrlBase en appsettings.json");

            string endpoint = configuration["Api:Endpoints:UrlBackendAPISlotSymbol"]
                ?? throw new Exception("No se encontró Api:Endpoints:UrlBackendAPISlotSymbol en appsettings.json");

            _url = baseUrl + endpoint;

            Console.WriteLine("URL cargada para SlotSymbol: " + _url);
        }

        // GET ALL
        public async Task<List<SlotSymbolResponse>> GetAllAsync()
        {
            var result = await _http.GetFromJsonAsync<List<SlotSymbolResponse>>(_url);
            return result ?? new List<SlotSymbolResponse>();
        }

        // GET BY ID
        public async Task<SlotSymbolResponse> GetByIdAsync(Guid id)
        {
            var result = await _http.GetFromJsonAsync<SlotSymbolResponse>($"{_url}/{id}");
            return result ?? new SlotSymbolResponse();
        }

        // CREATE
        public async Task CreateAsync(CreateSlotSymbolRequest request)
        {
            var response = await _http.PostAsJsonAsync(_url, request);
            response.EnsureSuccessStatusCode();
        }

        // UPDATE
        public async Task UpdateAsync(Guid id, UpdateSlotSymbolRequest request)
        {
            var response = await _http.PutAsJsonAsync($"{_url}/{id}", request);
            response.EnsureSuccessStatusCode();
        }

        // DELETE
        public async Task DeleteAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"{_url}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}