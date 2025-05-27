namespace IssueManager.Srevices
{
    public class CompanyApiServices
    {
        private readonly HttpClient _client;

        public CompanyApiServices()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44374/")
            };
        }

        public async Task<bool> CreateCompanyAsync(DummyCompanyDTO dto)
        {
            var response = await _client.PostAsJsonAsync("Companies/create", dto);
            return response.IsSuccessStatusCode;
        }
    }
}
