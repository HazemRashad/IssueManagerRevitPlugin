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
        [ObservableProperty] private int areaId;
        [ObservableProperty] private int projectId = 1;
        [ObservableProperty] private string createdByUserId = "user-id";
        [ObservableProperty] private string assignedToUserId;
        [ObservableProperty] private Priority priorityChoice;
        [ObservableProperty] private string? snapshotImagePath;

        public ObservableCollection<AssignLabelToIssueDto> SelectedLabels { get; set; } = new();

        [RelayCommand]
        private async Task SaveAsync()
        {
            var dto = new CreateIssueDto
            {
                Title = Title,
                Description = Description,
                AreaId = AreaId,
                ProjectId = ProjectId,
                CreatedByUserId = CreatedByUserId,
                AssignedToUserId = AssignedToUserId,
                CreatedAt = DateTime.UtcNow,
                Priority = PriorityChoice,
                Labels = SelectedLabels.ToList(),
                RevitElements = string.IsNullOrWhiteSpace(SnapshotImagePath)
                    ? new List<RevitElementDto>()
                    : new List<RevitElementDto>
                    {
                        new RevitElementDto
                        {
                            ElementId = "123",
                            ElementUniqueId = "ABC-123",
                            ViewpointCameraPosition = "0,0,0",
                            SnapshotImagePath = SnapshotImagePath
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
}
