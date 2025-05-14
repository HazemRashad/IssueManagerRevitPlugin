using Autodesk.Revit.Attributes;
using IssueManager.Resources;
using IssueManager.ViewModels;
using IssueManager.Views;
using Nice3point.Revit.Toolkit.External;

namespace IssueManager.Commands
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class LoadViewPointCommand : ExternalCommand
    {
        public override void Execute()
        {
            var viewModel = new LoadViewPointViewModel();
            var view = new LoadViewPointView(viewModel);
            WindowRevitHandler.ShowWindow(view);
        }
    }
}
