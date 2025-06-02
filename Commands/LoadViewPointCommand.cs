namespace IssueManager.Commands
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class LoadViewPointCommand : ExternalCommand
    {
        public override void Execute()
        {
            var httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:44374/") };
            var issueService = new IssueApiService(httpClient);
            var lookupService = new LookupApiService(httpClient);

            var viewModel = new LoadViewPointViewModel(lookupService, issueService);
            var view = new LoadViewPointView(viewModel); 
            WindowRevitHandler.ShowWindow(view); 
        }


    }
}
