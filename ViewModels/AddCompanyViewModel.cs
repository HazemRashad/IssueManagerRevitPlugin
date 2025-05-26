using IssueManager.Models;
using IssueManager.Srevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IssueManager.ViewModels
{
    [INotifyPropertyChanged]
    public sealed partial class AddCompanyViewModel
    {
        private readonly CompanyApiServices _apiService = new();

        [ObservableProperty] private string companyName = string.Empty;

        public IRelayCommand SaveCommand => new AsyncRelayCommand(SaveAsync);

        private async Task SaveAsync()
        {
            var dto = new DummyCompanyDTO
            {
                CompanyName = CompanyName
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
