[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class CommentCommand : ExternalCommand
{
    private readonly IssueDto _issue;
    private readonly Func<Task>? _onCommentAdded;

    public CommentCommand(IssueDto issue, Func<Task>? onCommentAdded = null)
    {
        _issue = issue;
        _onCommentAdded = onCommentAdded;
    }

    public override async void Execute()
    {
        var httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:44374/") };
        var commentApi = new CommentApiService(httpClient);

        var vm = new CommentViewModel(commentApi, _issue.IssueId);
        vm.OnCommentAdded = async () =>
        {
            if (_onCommentAdded != null)
                await _onCommentAdded();
        };

        var view = new CommentView(vm);
        WindowRevitHandler.ShowWindow(view);
    }
}

