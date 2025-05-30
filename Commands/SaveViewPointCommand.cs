using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueManager.Commands
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class SaveViewPointCommand : ExternalCommand
    {
        public override void Execute()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44374/")
            };
            var issueservice = new IssueApiService(httpClient);
            var viewModel = new SaveViewPointViewModel(issueservice);
            var view = new SaveViewPointView(viewModel);
            WindowRevitHandler.ShowWindow(view);
        }
    }
}
