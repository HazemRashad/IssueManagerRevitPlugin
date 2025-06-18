using DTOs.Comments;
using DTOs.Projects;
using IssueManager.Revit;
using Nice3point.Revit.Toolkit.External.Handlers;
using System.Collections.ObjectModel;
using System.Windows.Input;

public partial class IssuesListViewModel : ObservableObject
{
    private readonly LookupApiService _lookupService;
    private readonly IssueApiService _issueService;

    public IssuesListViewModel(LookupApiService lookupService, IssueApiService issueService)
    {
        _lookupService = lookupService;
        _issueService = issueService;

        LoadAllIssuesAsync();
        LoadUserProjectsAsync();
        LoadPriorities();

        ApplyFilterCommand = new RelayCommand(async () => await FilterIssuesAsync());
        ResetFilterCommand = new RelayCommand(async () =>
        {
            SelectedPriority = null;
            SelectedDate = null;
            SelectedProject = null;
            Issues.Clear();
            LoadAllIssuesAsync();
        });

        OpenIssueDetailsViewCommand = new RelayCommand<IssueDto>(OpenIssueDetails);
        AddIssueCommand = new RelayCommand(AddIssue);
    }

    [ObservableProperty] private ProjectDto? selectedProject;
    [ObservableProperty] private IssueDto? selectedIssue;
    [ObservableProperty] private Priority? selectedPriority;
    [ObservableProperty] private DateTime? selectedDate;

    public ObservableCollection<ProjectDto> Projects { get; set; } = new();
    public ObservableCollection<IssueDto> Issues { get; set; } = new();
    public ObservableCollection<string> Priorities { get; set; } = new();

    public ICommand AddIssueCommand { get; }
    public ICommand ApplyFilterCommand { get; }
    public ICommand ResetFilterCommand { get; }
    public ICommand OpenIssueDetailsViewCommand { get; }

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
    }

    private async Task LoadIssuesAsync()
    {
        if (SelectedProject == null) return;

        var issues = await _issueService.GetIssuesByProjectIdAsync(SelectedProject.ProjectId);
        Issues.Clear();
        foreach (var issue in issues)
            Issues.Add(issue);
    }
    private async Task LoadAllIssuesAsync()
    {
        var issues = await _issueService.GetAllAsync();
        Issues.Clear();
        if (issues == null) return;
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
        OpenIssueDetails(issue);
    }

    private void OpenIssueDetails(IssueDto? issue)
    {
        if (issue == null) return;

        var command = new IssueDetailsCommand(issue);
        command.Execute();
    }

    private void AddIssue()
    {
        var command = new SaveViewPointCommand();
        command.Execute();
    }

}
