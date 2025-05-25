using IssueManager.DTOs.Projects;

namespace IssueManager.DTOs.Companies
{
    public class CreateUserWithProjectsDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Role { get; set; } // Global role (Viewer, Editor, etc.)

        public List<ProjectAssignmentDto> ProjectAssignments { get; set; }
    }
}
