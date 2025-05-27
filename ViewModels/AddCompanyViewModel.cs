namespace IssueManager.ViewModels
{
    [INotifyPropertyChanged]
    public sealed partial class AddCompanyViewModel
    {
        private readonly CompanyApiService _apiService = new();

        private string companyName = string.Empty;

        public IRelayCommand SaveCommand => new AsyncRelayCommand(SaveAsync);

        private async Task SaveAsync()
        {

        }
    }
}
