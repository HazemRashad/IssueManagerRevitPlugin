using DTOs.Projects;

namespace IssueManager.Services
{
    public class ProjectApiService : ApiService
    {
        public ProjectApiService(HttpClient client) : base(client) { }

        public Task<List<ProjectDto>?> GetAllAsync()
            => GetAsync<List<ProjectDto>>(Projects.GetAll());

        public Task<ProjectDto?> GetByIdAsync(int id)
            => GetAsync<ProjectDto>(Projects.GetById(id));

        public Task<ProjectDto?> CreateAsync(CreateProjectDto dto)
            => PostAsync<CreateProjectDto, ProjectDto>(Projects.Create(), dto);

        public Task<HttpResponseMessage> UpdateAsync(int id, UpdateProjectDto dto)
            => PutAsync(Projects.Update(id), dto);

        public Task<HttpResponseMessage> DeleteAsync(int id)
            => DeleteAsync(Projects.Delete(id));

        public Task<List<ProjectOverviewDto>?> GetOverviewForUserAsync()
            => GetAsync<List<ProjectOverviewDto>>(Projects.OverviewForUser());

        public Task<List<ProjectOverviewDto>?> GetOverviewForCompanyAsync()
            => GetAsync<List<ProjectOverviewDto>>(Projects.OverviewForCompany());

        public Task<List<ProjectOverviewDto>?> GetOverviewForSubscriberAsync()
            => GetAsync<List<ProjectOverviewDto>>(Projects.OverviewForSubscriber());
    }
}
