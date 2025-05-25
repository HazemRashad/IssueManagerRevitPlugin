using Autodesk.Revit.Attributes;
using IssueManager.ViewModels;
using IssueManager.Views;
using Nice3point.Revit.Toolkit.External;

namespace IssueManager.Commands
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class SaveViewPointCommand : ExternalCommand
    {
        public override void Execute()
        {
            var viewModel = new AddCompanyViewModel();
            var view = new AddCompanyView(viewModel);
            view.ShowDialog();
        }
    }
}