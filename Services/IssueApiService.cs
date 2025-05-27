namespace IssueManager.Services
{
    public class IssueApiService : ApiService
    {
        public IssueApiService(HttpClient client) : base(client)
        {
        }

        public Task<List<IssueDto>?> GetAllAsync()
            => GetAsync<List<IssueDto>>(Issues.GetAll());

        public Task<IssueDto?> GetByIdAsync(int id)
            => GetAsync<IssueDto>(Issues.GetById(id));

        public Task<IssueDto?> CreateAsync(CreateIssueDto dto)
            => PostAsync<CreateIssueDto, IssueDto>(Issues.Create(), dto);

        public Task<HttpResponseMessage> UpdateAsync(int id, UpdateIssueDto dto)
            => PutAsync(Issues.Update(id), dto);

        public Task<HttpResponseMessage> DeleteAsync(int id)
            => DeleteAsync(Issues.Delete(id));
    }
}
