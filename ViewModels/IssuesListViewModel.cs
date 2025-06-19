using DTOs.Comments;
using DTOs.Projects;
using DTOs.Issues;
using IssueManager.Revit;
using Nice3point.Revit.Toolkit.External.Handlers;
using System.Collections.ObjectModel;
using System.Windows.Input;

public partial class IssuesListViewModel : ObservableObject
{
    private readonly LookupApiService _lookupService;
    private readonly IssueApiService _issueService;
    private readonly AsyncEventHandler _asyncEventHandler = new();

    // 🔹 All issues used for search/filtering
    private List<IssueDto> _allIssues = new();

    public IssuesListViewModel(LookupApiService lookupService, IssueApiService issueService)
    {
        _lookupService = lookupService;
        _issueService = issueService;

        LoadUserProjectsAsync();
        LoadPriorities();
        LoadAllIssuesAsync();

        ApplyFilterCommand = new RelayCommand(async () => await ApplyFilterAsync());
        ResetFilterCommand = new RelayCommand(async () => await ResetFilterAsync());
        OpenIssueDetailsViewCommand = new RelayCommand<IssueDto>(OpenIssueDetails);
        AddIssueCommand = new RelayCommand(AddIssue);
    }

    // 🔹 Observable Bindings
    [ObservableProperty] private ProjectDto? selectedProject;
    [ObservableProperty] private IssueDto? selectedIssue;
    [ObservableProperty] private Priority? selectedPriority;
    [ObservableProperty] private DateTime? selectedDate;
    [ObservableProperty] private string searchQuery;
    [ObservableProperty] private ObservableCollection<IssueDto> issues = new();

    public ObservableCollection<ProjectDto> Projects { get; set; } = new();
    public ObservableCollection<string> Priorities { get; set; } = new();

    public ICommand AddIssueCommand { get; }
    public ICommand ApplyFilterCommand { get; }
    public ICommand ResetFilterCommand { get; }
    public ICommand OpenIssueDetailsViewCommand { get; }

    // 🔹 Reactive search
    partial void OnSearchQueryChanged(string value) => RefreshView();

    partial void OnSelectedProjectChanged(ProjectDto? value)
    {
        _ = LoadAndFilterIssuesAsync();
    }

    // 🔹 Load all projects for filter ComboBox
    private async Task LoadUserProjectsAsync()
    {
        if (string.IsNullOrWhiteSpace(AppSession.UserId)) return;

        var projects = await _lookupService.GetProjectsByUserIdAsync(AppSession.UserId);
        Projects.Clear();
        foreach (var project in projects)
            Projects.Add(project);
    }

    private void LoadPriorities()
    {
        Priorities.Clear();
        foreach (var name in Enum.GetNames(typeof(Priority)))
            Priorities.Add(name);
    }

    // 🔹 Load all issues
    private async Task LoadAllIssuesAsync()
    {
        var issues = await _issueService.GetAllAsync();
        _allIssues = issues?.ToList() ?? new();
        RefreshView();
    }

    // 🔹 Load issues by project
    private async Task LoadAndFilterIssuesAsync()
    {
        if (SelectedProject == null) return;

        var issues = await _issueService.GetIssuesByProjectIdAsync(SelectedProject.ProjectId);
        _allIssues = issues?.ToList() ?? new();
        RefreshView();
    }

    // 🔹 Filters + Search Applied Here
    private void RefreshView()
    {
        var filtered = _allIssues.AsEnumerable();

        if (SelectedPriority != null)
            filtered = filtered.Where(i => i.Priority == SelectedPriority);

        if (SelectedDate != null)
            filtered = filtered.Where(i => i.CreatedAt.Date == SelectedDate.Value.Date);

        if (!string.IsNullOrWhiteSpace(SearchQuery))
            filtered = filtered.Where(i =>
                i.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                (i.Description?.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ?? false));

        Issues = new ObservableCollection<IssueDto>(filtered);
    }

    // 🔹 Called by UI
    private async Task ApplyFilterAsync()
    {
        await LoadAndFilterIssuesAsync();
    }

    private async Task ResetFilterAsync()
    {
        SelectedPriority = null;
        SelectedDate = null;
        SelectedProject = null;
        SearchQuery = string.Empty;

        await LoadAllIssuesAsync();
    }

    // 🔹 Issue Details
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

    // 🔹 Add New Issue
    private void AddIssue()
    {
        var command = new SaveViewPointCommand();
        command.Execute();
    }
}
