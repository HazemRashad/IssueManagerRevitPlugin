namespace IssueManager.ViewModels
{
    [INotifyPropertyChanged]
    public sealed partial class AddCompanyViewModel
    {
        private readonly CompanyApiServices _apiService = new();

        private string companyName = string.Empty;

        public IRelayCommand SaveCommand => new AsyncRelayCommand(SaveAsync);

        private async Task SaveAsync()
        {
            var dto = new DummyCompanyDTO
            {
                CompanyName = companyName
            };

            bool result = await _apiService.CreateCompanyAsync(dto);
            if (result)
            {
                MessageBox.Show("Company created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Failed to create Company.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
