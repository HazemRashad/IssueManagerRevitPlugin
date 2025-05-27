using DTOs.Companies;

namespace IssueManager.Srevices
{
    public class CompanyApiService : ApiService
    {
        public CompanyApiService(HttpClient client) : base(client) { }

        public Task<List<CompanyDto>?> GetAllAsync()
            => GetAsync<List<CompanyDto>>(Companies.GetAll());

        public Task<CompanyDto?> GetByIdAsync(int id)
            => GetAsync<CompanyDto>(Companies.GetById(id));

        public Task<CompanyDto?> CreateAsync(CreateCompanyDto dto)
            => PostAsync<CreateCompanyDto, CompanyDto>(Companies.Create(), dto);

        public Task<HttpResponseMessage> CreateWithAdminAsync(CreateCompanyWithAdminDto dto)
            => PostAsync(Companies.CreateWithAdmin(), dto);

        public Task<HttpResponseMessage> UpdateAsync(int id, UpdateCompanyDto dto)
            => PutAsync(Companies.Update(id), dto);

        public Task<HttpResponseMessage> DeleteAsync(int id)
            => DeleteAsync(Companies.Delete(id));

        public Task<List<CompanyOverviewDto>?> GetOverviewForUserAsync()
            => GetAsync<List<CompanyOverviewDto>>(Companies.GetOverviewForUser());
    }
}
