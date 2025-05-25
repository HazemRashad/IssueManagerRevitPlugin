namespace IssueManager.DTOs
{
    public class UserOverviewDto
    {
        public string Id {get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string Role { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ProjectsIncludedCount { get; set; }
        public int IssuesCreatedCount { get; set; }
    }
}
