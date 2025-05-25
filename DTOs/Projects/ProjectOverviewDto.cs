namespace IssueManager.DTOs
{
    public class ProjectOverviewDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int IssuesCount { get; set; }
        public List<string> CompanyNames { get; set; }
        public string UserRoleInProject { get; set; }
    }
}
