using DTOs.Areas;
using DTOs.Labels;
using DTOs.Projects;
using DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueManager.Constants
{
    public class LookupApiService : ApiService
    {

        public LookupApiService(HttpClient client) : base(client) { }


        public Task<List<AreaDto>> GetAreasByProjectIdAsync(int projectId)
            => GetAsync<List<AreaDto>>($"api/area/project/{projectId}");

        public Task<List<UserDto>> GetUsersByProjectIdAsync(int projectId)
            => GetAsync<List<UserDto>>($"api/users/project-users/{projectId}");

        public Task<List<LabelDto>> GetLabelsByProjectIdAsync(int projectId)
            => GetAsync<List<LabelDto>>($"api/labels/project/{projectId}/labels");
        public Task<List<ProjectDto>> GetProjectsByUserIdAsync(string userId)
    => GetAsync<List<ProjectDto>>($"api/projects/user/{userId}");
    }

}