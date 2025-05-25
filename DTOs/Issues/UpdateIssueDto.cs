namespace IssueManager.DTOs.Issues
{
    public class UpdateIssueDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssignedToUserId { get; set; }
    }
}
