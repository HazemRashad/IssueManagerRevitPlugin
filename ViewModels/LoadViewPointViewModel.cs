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
                Issues.Add(issue);
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