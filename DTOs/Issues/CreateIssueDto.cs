namespace IssueManager.DTOs.Issues
{
    public class CreateIssueDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<IssueLabel> Labels { get; set; }
        public ICollection<RevitElement> RevitElements { get; set; }
        public int AreaId { get; set; }
        public Priority Priority { get; set; }
        public string? AssignedToUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProjectId { get; set; }
        public string CreatedByUserId { get; set; }

    }
}
