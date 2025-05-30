namespace IssueManager.Commands
{
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class LoginViewPointCommand : ExternalCommand
    {
        public override void Execute()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44374/") // أو حسب عنوان الـ API
            };
            var loginservice = new LoginApiService(httpClient);
            var viewModel = new LoginViewModel(loginservice);
            var view = new LoginView(viewModel);
            view.ShowDialog();

        }
    }
}