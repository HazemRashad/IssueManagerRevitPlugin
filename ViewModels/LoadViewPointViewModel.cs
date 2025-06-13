using DTOs.Comments;
using DTOs.Projects;
using DTOs.Issues;
using IssueManager.Revit;
using Nice3point.Revit.Toolkit.External.Handlers;
using System.Collections.ObjectModel;
using System.Windows.Input;

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
            LoadPriorities();

            ApplyFilterCommand = new RelayCommand(async () => await FilterIssuesAsync());
            ResetFilterCommand = new RelayCommand(async () =>
            {
                SelectedPriority = null;
                SelectedDate = null;
                SelectedProject = null;
                await LoadUserProjectsAsync();
                Issues.Clear();
            });
        }

        [ObservableProperty] private ProjectDto? selectedProject;
        [ObservableProperty] private IssueDto? selectedIssue;
        [ObservableProperty] private Priority? selectedPriority;
        [ObservableProperty] private DateTime? selectedDate;

        public ObservableCollection<ProjectDto> Projects { get; set; } = new();
        public ObservableCollection<IssueDto> Issues { get; set; } = new();
        public ObservableCollection<CommentDto> Comments { get; set; } = new();
        public ObservableCollection<string> Priorities { get; set; } = new();

        public ICommand ApplyFilterCommand { get; }
        public ICommand ResetFilterCommand { get; }

        private void LoadPriorities()
        {
            Priorities.Clear();
            foreach (var name in Enum.GetNames(typeof(Priority)))
            {
                Priorities.Add(name);
            }
        }

        private async Task LoadUserProjectsAsync()
        {
            if (string.IsNullOrWhiteSpace(AppSession.UserId)) return;

            var projects = await _lookupService.GetProjectsByUserIdAsync(AppSession.UserId);
            Projects.Clear();
            foreach (var project in projects)
                Projects.Add(project);

            SelectedProject = null;
        }

        private async Task LoadIssuesAsync()
        {
            if (SelectedProject == null) return;

            var issues = await _issueService.GetIssuesByProjectIdAsync(SelectedProject.ProjectId);
            Issues.Clear();
            foreach (var issue in issues)
                Issues.Add(issue);
        }

        private async Task FilterIssuesAsync()
        {
            if (SelectedProject == null) return;

            var issues = await _issueService.GetIssuesByProjectIdAsync(SelectedProject.ProjectId);
            var filtered = issues.AsEnumerable();

            if (SelectedPriority != null)
                filtered = filtered.Where(i => i.Priority == SelectedPriority);

            if (SelectedDate != null)
                filtered = filtered.Where(i => i.CreatedAt.Date == SelectedDate.Value.Date);

            Issues.Clear();
            foreach (var issue in filtered)
                Issues.Add(issue);
        }

        private readonly AsyncEventHandler _asyncEventHandler = new();

        [RelayCommand]
        private async Task SetSelectedIssue(IssueDto issue)
        {
            SelectedIssue = issue;
            if (SelectedIssue is null) return;
            await _asyncEventHandler.RaiseAsync(_ =>
            {
                RevitIssue.NavigateToViewPointAndSelectElements(issue);
            });
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
