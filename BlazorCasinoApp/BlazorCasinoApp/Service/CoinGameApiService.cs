using Chaos.BlazorCasinoApp.IApiService;
using Chaos.Shared.RequestEntity;
using Chaos.Shared.ResponseEntity;
using System.Net.Http;
using System.Net.Http.Json;

namespace Chaos.BlazorCasinoApp.Service
{
    public class CoinGameApiService : ICoinGameService
    {
        private readonly HttpClient _http;
        private readonly string _url;

        public CoinGameApiService(HttpClient http, IConfiguration configuration)
        {
            _http = http;

            string baseUrl = configuration["Api:UrlBase"] ?? throw new Exception("No se encontró Api:UrlBase en appsettings.json");

            string endpoint = configuration["Api:Endpoints:UrlBackendAPICoinFlip"] ?? throw new Exception("No se encontró Api:Endpoints:UrlBackendAPICoinFlip en appsettings.json");


            _url = baseUrl + endpoint;

            Console.WriteLine("URL cargada para obtener Config de Coin Game: " + _url);
        }

        //GET ALL
        public async Task<List<CoinGameResponse>> GetAllAsync()
        {
            var result = await _http.GetFromJsonAsync<List<CoinGameResponse>>(_url);
            return result ?? new List<CoinGameResponse>();
        }

        // GET BY ID
        public async Task<CoinGameResponse> GetByIdAsync(Guid id)
        {
            var result = await _http.GetFromJsonAsync<CoinGameResponse>($"{_url}/{id}");
            return result ?? new CoinGameResponse();
        }

        //CREATE
        public async Task CreateAsync(CoinGameRequest request)
        {
            var response = await _http.PostAsJsonAsync(_url, request);
            response.EnsureSuccessStatusCode();
        }

        // UPDATE
        public async Task UpdateAsync(Guid id, CoinGameRequest request)
        {
            var response = await _http.PutAsJsonAsync($"{_url}/{id}", request);
            response.EnsureSuccessStatusCode();
        }

        // DELETE
        public async Task DeleteAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"{_url}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                //Lee el mensaje REAL del backend
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(errorMessage);
            }
        }

        //TOGGLE ACTIVE
        public async Task ToggleActiveAsync(Guid id)
        {
            var response = await _http.PatchAsync($"{_url}/{id}/toggle", null);
            response.EnsureSuccessStatusCode();
        }

        //GET ACTIVE
        public async Task<CoinGameResponse> GetActiveAsync()
        {
            var result = await _http.GetFromJsonAsync<CoinGameResponse>($"{_url}/active");
            return result ?? new CoinGameResponse();
        }
    }
}
