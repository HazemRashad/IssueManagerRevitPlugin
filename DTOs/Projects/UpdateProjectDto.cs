namespace IssueManager.DTOs
{
    public class UpdateProjectDto
    {
        public string ProjectName { get; set; }
        public int CompanyId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
