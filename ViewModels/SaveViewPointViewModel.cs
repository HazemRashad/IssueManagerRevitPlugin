using DTOs.Areas;
using DTOs.IssueLabel;
using DTOs.Labels;
using DTOs.Projects;
using DTOs.RevitElements;
using DTOs.Snapshots;
using DTOs.Users;
using IssueManager.Constants;
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

            LoadUserProjectsAsync();
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
        [ObservableProperty] private ProjectDto? selectedProject;

        public ObservableCollection<ProjectDto> UserProjects { get; set; } = new();
        public ObservableCollection<AreaDto> Areas { get; set; } = new();
        public ObservableCollection<UserDto> Users { get; set; } = new();
        public ObservableCollection<LabelDto> Labels { get; set; } = new();
        public static Action CloseAction { get; internal set; }

        private async void LoadUserProjectsAsync()
        {
            if (string.IsNullOrWhiteSpace(AppSession.UserId)) return;

            var projects = await _lookupService.GetProjectsByUserIdAsync(AppSession.UserId);
            UserProjects.Clear();
            foreach (var p in projects) UserProjects.Add(p);
        }

        partial void OnSelectedProjectChanged(ProjectDto? value)
        {
            if (value is null) return;
            projectId = value.ProjectId;
            LoadLookupsByProjectIdAsync(projectId);
        }

        private async void LoadLookupsByProjectIdAsync(int projectId)
        {
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
                ProjectId = SelectedProject?.ProjectId ?? 0,
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
            }
         },
                       Snapshot = !string.IsNullOrWhiteSpace(SnapshotImagePath)
                ? new SnapshotDto
                {
                    Path = SnapshotImagePath,
                    CreatedAt = DateTime.UtcNow
                }
                : null
                   };


            var created = await _issueService.CreateAsync(dto);
            if (created is not null)
            {
                MessageBox.Show("Issue Created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ResetForm();
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

        private async Task ExportSnapshotAsync()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), $"Snapshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            string snapshotPath = RevitIssue.ExportSnapshot(tempPath);

            if (string.IsNullOrWhiteSpace(snapshotPath))
            {
                MessageBox.Show("Failed to export snapshot.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using var multipart = new MultipartFormDataContent();
                multipart.Add(new StreamContent(File.OpenRead(snapshotPath)), "file", Path.GetFileName(snapshotPath));

                using var client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44374/");
                var response = await client.PostAsync("/api/Snapshot/upload", multipart);

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Upload failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var rawPath = await response.Content.ReadAsStringAsync();

                // تأكد إنك ماضفتش الـ base URL مرتين من API
                if (!rawPath.StartsWith("http"))
                {
                    rawPath = "https://localhost:44374/" + rawPath.TrimStart('/');
                }

                SnapshotImagePath = rawPath;

                MessageBox.Show("Snapshot uploaded and linked!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error uploading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            SelectedProject = null;
        }

        [RelayCommand]
        private void Cancel() => CloseWindow();

        [RelayCommand]
        private void ClearSnapshot()
        {
            SnapshotImagePath = null;
        }
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
