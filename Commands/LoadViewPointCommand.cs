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
