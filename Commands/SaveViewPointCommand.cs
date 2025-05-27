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