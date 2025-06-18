namespace IssueManager.Commands
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class IssuesListCommand : ExternalCommand
    {
        public override void Execute()
        {
            var httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:44374/") };
            var issueService = new IssueApiService(httpClient);
            var lookupService = new LookupApiService(httpClient);

            var viewModel = new IssuesListViewModel(lookupService, issueService);
            var view = new IssuesListView(viewModel);
            WindowRevitHandler.ShowWindow(view);
        }


    }
}
