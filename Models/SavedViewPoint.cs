namespace IssueManager.Models
{
    public class SavedViewPoint
    {
        public string TaskName { get; set; }
        public string Comment { get; set; }
        public string Type { get; set; }
        public string MasterId { get; set; }

        public List<string> SelectedElementsIds { get; set; }
        public string ViewName { get; set; }
        public string ViewOrientationJson { get; set; }
        public string SectionBoxJson { get; set; }
        public string SnapshotBase64 { get; set; }

        public string AssignedTo { get; set; }
        public bool Flagged { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
