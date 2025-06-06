using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DTOs.Issues;
using DTOs.Projects;
using DTOs.Comments;
using IssueManager.Constants;
using IssueManager.Services;
using System.Collections.ObjectModel;

namespace IssueManager.ViewModels
{
    public partial class LoadViewPointViewModel : ObservableObject
    {
        private readonly LookupApiService _lookupService;
        private readonly IssueApiService _issueService;

        public LoadViewPointViewModel(LookupApiService lookupService, IssueApiService issueService)
        {
            _lookupService = lookupService;
            _issueService = issueService;

            LoadUserProjectsAsync();
        }

        [ObservableProperty] private ProjectDto? selectedProject;
        [ObservableProperty] private IssueDto? selectedIssue;

        public ObservableCollection<ProjectDto> UserProjects { get; set; } = new();
        public ObservableCollection<IssueDto> Issues { get; set; } = new();
        public ObservableCollection<CommentDto> Comments { get; set; } = new();

        private async void LoadUserProjectsAsync()
        {
            if (string.IsNullOrWhiteSpace(AppSession.UserId)) return;

            var projects = await _lookupService.GetProjectsByUserIdAsync(AppSession.UserId);
            UserProjects.Clear();
            foreach (var project in projects)
                UserProjects.Add(project);
        }

        partial void OnSelectedProjectChanged(ProjectDto? value)
        {
            if (value is null) return;
            LoadIssuesByProjectIdAsync(value.ProjectId);
        }

        [RelayCommand]
        private void SetSelectedIssue(IssueDto issue)
        {
            SelectedIssue = issue;
        }

        private async void LoadIssuesByProjectIdAsync(int projectId)
        {
            var issues = await _issueService.GetIssuesByProjectIdAsync(projectId);
            Issues.Clear();

            foreach (var issue in issues)
            {
                if (!string.IsNullOrWhiteSpace(issue.Snapshot?.Path))
                {
                    try
                    {
                        // Step 1: جهز المسار المحلي المؤقت
                        var fileName = Path.GetFileName(issue.Snapshot.Path);
                        var tempPath = Path.Combine(Path.GetTempPath(), fileName);

                        // Step 2: حمل الصورة من السيرفر
                        using var httpClient = new HttpClient();
                        var imageBytes = await httpClient.GetByteArrayAsync(issue.Snapshot.ImagePath);

                        // Step 3: خزّن الصورة مؤقتًا
                        File.WriteAllBytes(tempPath, imageBytes);

                        // Step 4: اربط المسار المؤقت بالـ LocalImagePath
                        issue.Snapshot.LocalImagePath = tempPath;
                    }
                    catch
                    {
                        issue.Snapshot.LocalImagePath = null;
                    }
                }

                Issues.Add(issue);
            }
        }



        partial void OnSelectedIssueChanged(IssueDto? value)
        {
            Comments.Clear();
            if (value?.Comments != null)
            {
                foreach (var comment in value.Comments)
                    Comments.Add(comment);
            }
        }
    }
}