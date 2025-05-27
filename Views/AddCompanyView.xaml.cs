using IssueManager.ViewModels;
using System.Windows;

namespace IssueManager.Views
{
    /// <summary>
    /// Interaction logic for AddCompanyView.xaml
    /// </summary>
    public partial class AddCompanyView : Window
    {
        public AddCompanyView(AddCompanyViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
