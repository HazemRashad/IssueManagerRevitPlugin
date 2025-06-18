using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueManager.Commands
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class IssueDetailsCommand : ExternalCommand
    {
        private readonly IssueDto _issue;

        public IssueDetailsCommand(IssueDto issue)
        {
            _issue = issue;
        }

        public override async void Execute()
        {
            var httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:44374/") };
            var issueService = new IssueApiService(httpClient);

            var viewModel = new IssueDetailsViewModel(issueService);
            await viewModel.LoadIssueAsync(_issue.IssueId);

            var view = new IssueDetailsView(viewModel);
            WindowRevitHandler.ShowWindow(view);
        }
    }
}
