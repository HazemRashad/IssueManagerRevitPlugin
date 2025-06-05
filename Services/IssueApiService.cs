namespace IssueManager.Services
{
    public class IssueApiService : ApiService
    {
        protected readonly HttpClient _client;
        public IssueApiService(HttpClient client) : base(client)
        {
            _client = client;
        }

        public Task<List<IssueDto>?> GetAllAsync()
            => GetAsync<List<IssueDto>>(Issues.GetAll());

        public Task<IssueDto?> GetByIdAsync(int id)
            => GetAsync<IssueDto>(Issues.GetById(id));

        public async Task<IssueDto?> CreateAsync(CreateIssueDto dto)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("api/issues/create", dto);
                string body = await response.Content.ReadAsStringAsync();

                MessageBox.Show($"Status: {response.StatusCode}\nBody:\n{body}", "DEBUG");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<IssueDto>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"EX: {ex.Message}");
                return null;
            }
        }

        public Task<HttpResponseMessage> UpdateAsync(int id, UpdateIssueDto dto)
            => PutAsync(Issues.Update(id), dto);

        public Task<HttpResponseMessage> DeleteAsync(int id)
            => DeleteAsync(Issues.Delete(id));

        public async Task<List<IssueDto>> GetIssuesByProjectIdAsync(int projectId)
        {
            return await GetAsync<List<IssueDto>>($"api/issues/project-issues/{projectId}");
        }


    }
}