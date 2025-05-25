using DTOs.Issues;
using System.Net.Http;
using System.Net.Http.Json;


namespace IssueManager.Srevices
{
    public class ApiService
    {
        private readonly HttpClient _client;

        public ApiService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7042/api/")
            };
        }

        public async Task<bool> CreateIssueAsync(CreateIssueDto dto)
        {
            var response = await _client.PostAsJsonAsync("issues", dto);
            return response.IsSuccessStatusCode;
        }
    }
}
