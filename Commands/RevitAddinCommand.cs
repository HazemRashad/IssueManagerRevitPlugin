using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueManager.Commands
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class RevitAddinCommand : ExternalCommand
    {
        public override void Execute()
        {
            var viewModel = new RevitAddInViewModel();
            var view = new RevitAddinView(viewModel);
            WindowRevitHandler.ShowWindow(view);
        }
    }
}
