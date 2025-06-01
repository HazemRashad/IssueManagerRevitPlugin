using DTOs.Areas;
using DTOs.IssueLabel;
using DTOs.Labels;
using DTOs.RevitElements;
using DTOs.Users;
using IssueManager.Revit;
using Nice3point.Revit.Toolkit.External.Handlers;
using System.Collections.ObjectModel;

namespace IssueManager.ViewModels
{
    public partial class SaveViewPointViewModel : ObservableObject
    {
        private readonly IssueApiService _issueService;
        private readonly LookupApiService _lookupService;

        public SaveViewPointViewModel(IssueApiService issueService, LookupApiService lookupService)
        {
            _issueService = issueService;
            _lookupService = lookupService;

            LoadLookupsAsync();
        }

        [ObservableProperty] private string title;
        [ObservableProperty] private string description;
        [ObservableProperty] private int areaId;
        [ObservableProperty] private int projectId;
        [ObservableProperty] private string createdByUserId;
        [ObservableProperty] private string assignedToUserId;
        [ObservableProperty] private Priority priorityChoice;
        [ObservableProperty] private string? snapshotImagePath;
        [ObservableProperty] private LabelDto? selectedLabel;

        public ObservableCollection<AreaDto> Areas { get; set; } = new();
        public ObservableCollection<UserDto> Users { get; set; } = new();
        public ObservableCollection<LabelDto> Labels { get; set; } = new();
        public static Action CloseAction { get; internal set; }

        private async void LoadLookupsAsync()
        {
            MessageBox.Show($"DEBUG:\nProjectId = {AppSession.ProjectId}\nUserId = {AppSession.UserId}");
            projectId = AppSession.ProjectId ?? 0;

            var areas = await _lookupService.GetAreasByProjectIdAsync(projectId);
            var users = await _lookupService.GetUsersByProjectIdAsync(projectId);
            var labels = await _lookupService.GetLabelsByProjectIdAsync(projectId);

            Areas.Clear();
            foreach (var a in areas) Areas.Add(a);

            Users.Clear();
            foreach (var u in users) Users.Add(u);

            Labels.Clear();
            foreach (var l in labels) Labels.Add(l);
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            var dto = new CreateIssueDto
            {
                Title = Title,
                Description = Description,
                AreaId = AreaId,
                ProjectId = AppSession.ProjectId ?? 0,
                CreatedByUserId = AppSession.UserId,
                AssignedToUserId = AssignedToUserId,
                CreatedAt = DateTime.UtcNow,
                Priority = PriorityChoice,
                Labels = SelectedLabel is not null
                    ? new List<AssignLabelToIssueDto> { new AssignLabelToIssueDto { LabelId = SelectedLabel.LabelId } }
                    : new List<AssignLabelToIssueDto>(),
                RevitElements = string.IsNullOrWhiteSpace(SnapshotImagePath)
                    ? new List<IssueRevitElementDto>()
                    : new List<IssueRevitElementDto>
                    {
                        new IssueRevitElementDto
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
                ResetForm(); // 🧹
            }
            else
            {
                MessageBox.Show("Failed to create issue.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private readonly AsyncEventHandler _asyncEventHandler = new();

        [RelayCommand]
        private async Task SectionBoxAsync()
        {
            await _asyncEventHandler.RaiseAsync(_ =>
            {
                RevitIssue.IsolateSelectionInSectionBox();
            });
        }

        [RelayCommand]
        private void ExportSnapshot()
        {
            string outputPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string snapshotPath = RevitIssue.ExportSnapshot(outputPath);

            if (!string.IsNullOrWhiteSpace(snapshotPath))
            {
                MessageBox.Show($"Snapshot saved to: {snapshotPath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Failed to export snapshot.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetForm()
        {
            Title = string.Empty;
            Description = string.Empty;
            AreaId = 0;
            AssignedToUserId = string.Empty;
            PriorityChoice = default;
            SnapshotImagePath = null;
            SelectedLabel = null;
        }

        [RelayCommand]
        private void Cancel() => CloseWindow();

        private void CloseWindow()
        {
            CloseAction?.Invoke();
        }
    }

    public static class PriorityValues
    {
        public static readonly List<Priority> All = Enum.GetValues(typeof(Priority)).Cast<Priority>().ToList();
    }
}
