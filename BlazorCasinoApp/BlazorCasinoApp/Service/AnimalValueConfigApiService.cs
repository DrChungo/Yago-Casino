using Chaos.BlazorCasinoApp.IApiService;
using Chaos.Shared.RequestEntity.AnimalValue;
using Chaos.Shared.ResponseEntity;
using System.Net.Http.Json;

namespace Chaos.BlazorCasinoApp.Service
{
    public class AnimalValueConfigApiService : IAnimalValueConfigService
    {
        private readonly HttpClient _http;
        private readonly string _url;

        public AnimalValueConfigApiService(HttpClient http, IConfiguration configuration)
        {
            _http = http;

            string baseUrl = configuration["Api:UrlBase"]
                ?? throw new Exception("No se encontró Api:UrlBase en appsettings.json");

            string endpoint = configuration["Api:Endpoints:UrlBackendAPIAnimalValueConfig"]
                ?? throw new Exception("No se encontró Api:Endpoints:UrlBackendAPIAnimalValueConfig en appsettings.json");

            _url = baseUrl + endpoint;
        }

        // GET ALL 
        public async Task<List<AnimalValueResponse>> GetAllAsync()
        {
            var result = await _http.GetFromJsonAsync<List<AnimalValueResponse>>(_url);
            return result ?? new List<AnimalValueResponse>();
        }

        // GET BY ID — con SVGs, solo al hacer clic 
        public async Task<AnimalValueResponse?> GetByIdAsync(Guid id)
        {
            return await _http.GetFromJsonAsync<AnimalValueResponse>($"{_url}/{id}");
        }

        public async Task<AnimalValueResponse> CreateAsync(CreateAnimalValueRequest request)
        {
            var response = await _http.PostAsJsonAsync(_url, request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<AnimalValueResponse>();
            return result ?? new AnimalValueResponse();
        }

        // UPDATE
        public async Task<AnimalValueResponse?> UpdateAsync(Guid id, UpdateAnimalValueRequest request)
        {
            var response = await _http.PutAsJsonAsync($"{_url}/{id}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AnimalValueResponse>();
        }

        // DELETE 
        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _http.DeleteAsync($"{_url}/{id}");
            return response.IsSuccessStatusCode;
        }

        // GET SVG FILES
        public async Task<List<string>> GetSvgFilesAsync()
        {
            var result = await _http.GetFromJsonAsync<List<string>>($"{_url}/svg-files");
            return result ?? new List<string>();
        }
    }
}