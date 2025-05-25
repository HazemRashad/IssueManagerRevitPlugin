namespace IssueManager.DTOs.Companies
{
    public class CompanyOverviewDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        public int UsersCount { get; set; }
        public int ProjectsCount { get; set; }
        public int IssuesCount { get; set; }
    }
}
