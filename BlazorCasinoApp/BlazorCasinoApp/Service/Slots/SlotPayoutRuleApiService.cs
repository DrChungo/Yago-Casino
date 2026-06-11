using Chaos.BlazorCasinoApp.IApiService;
using Chaos.BlazorCasinoApp.IApiService.Slots;
using Chaos.Shared.RequestEntity.SlotGame;
using Chaos.Shared.ResponseEntity.SlotGame;
using System.Net.Http.Json;

namespace Chaos.BlazorCasinoApp.Service.Slots
{
    public class SlotPayoutRuleApiService : ISlotPayoutRuleService
    {
        private readonly HttpClient _http;
        private readonly string _url;

        public SlotPayoutRuleApiService(HttpClient http, IConfiguration configuration)
        {
            _http = http;

            string baseUrl = configuration["Api:UrlBase"]
                ?? throw new Exception("No se encontró Api:UrlBase en appsettings.json");

            string endpoint = configuration["Api:Endpoints:UrlBackendAPISlotPayoutRule"]
                ?? throw new Exception("No se encontró Api:Endpoints:UrlBackendAPISlotPayoutRule en appsettings.json");

            _url = baseUrl + endpoint;

            Console.WriteLine("URL cargada para SlotPayoutRule: " + _url);
        }

        // GET ALL
        public async Task<List<SlotPayoutRuleResponse>> GetAllAsync()
        {
            var result = await _http.GetFromJsonAsync<List<SlotPayoutRuleResponse>>(_url);
            return result ?? new List<SlotPayoutRuleResponse>();
        }

        // GET BY ID
        public async Task<SlotPayoutRuleResponse> GetByIdAsync(Guid id)
        {
            var result = await _http.GetFromJsonAsync<SlotPayoutRuleResponse>($"{_url}/{id}");
            return result ?? new SlotPayoutRuleResponse();
        }

        // CREATE
        public async Task CreateAsync(CreateSlotPayoutRuleRequest request)
        {
            var response = await _http.PostAsJsonAsync(_url, request);
            response.EnsureSuccessStatusCode();
        }

        // UPDATE
        public async Task UpdateAsync(Guid id, UpdateSlotPayoutRuleRequest request)
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