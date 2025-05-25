namespace IssueManager.DTOs.Users
{
    public class AssignUserToProjectDto
    {
        public int ProjectId { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}
