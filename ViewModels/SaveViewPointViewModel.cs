using DTOs.IssueLabel;
using DTOs.RevitElements;
using System.Collections.ObjectModel;

namespace IssueManager.ViewModels
{
    public partial class SaveViewPointViewModel : ObservableObject
    {
        private readonly IssueApiService _issueService;

        public SaveViewPointViewModel(IssueApiService issueService)
        {
            _issueService = issueService;
        }

        [ObservableProperty] private string title;
        [ObservableProperty] private string description;
        [ObservableProperty] private int areaId=1;
        [ObservableProperty] private int projectId = 1;
        [ObservableProperty] private string createdByUserId = "user-id";
        [ObservableProperty] private string assignedToUserId="Hazem";
        [ObservableProperty] private Priority priorityChoice;
        [ObservableProperty] private string? snapshotImagePath=null;

        public ObservableCollection<AssignLabelToIssueDto> SelectedLabels { get; set; } = new();

        [RelayCommand]
        private async Task SaveAsync()
        {
            var dto = new CreateIssueDto
            {
                Title = "Test Issue Title",
                Description = "This is a sample description for the issue.",
                AreaId = 1, // ID منطقي من الـ API
                ProjectId = 1,
                CreatedByUserId = "fixed-user-id-123",
                AssignedToUserId = "Hazem-456", // يفضل ID حقيقي للمستخدم
                CreatedAt = DateTime.UtcNow,
                Priority = Priority.Critical, // أو Priority.Normal أو أي قيمة من enum
                Labels = new List<AssignLabelToIssueDto>
        {
            new AssignLabelToIssueDto { LabelId = 1 },
            new AssignLabelToIssueDto { LabelId = 2 }
        },
                RevitElements = new List<RevitElementDto>
        {
            new RevitElementDto
            {
                ElementId = "123",
                ElementUniqueId = "ABC-123",
                ViewpointCameraPosition = "0,0,0",
                SnapshotImagePath = @"C:\Temp\snapshot.png" // حط أي path مؤقت
            }
        }
            };

            var created = await _issueService.CreateAsync(dto);
            if (created is not null)
            {
                MessageBox.Show("Issue Created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseWindow();
            }
            else
            {
                MessageBox.Show("Failed to create issue.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel() => CloseWindow();

        private void CloseWindow()
        {
            System.Windows.Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)?.Close();
        }

    }
    public static class PriorityValues
    {
        public static readonly List<Priority> All = Enum.GetValues(typeof(Priority)).Cast<Priority>().ToList();
    }
}
