using System.Windows;
using IssueManager.ViewModels;

namespace IssueManager.Views
{
    /// <summary>
    /// Interaction logic for LoadViewPointView.xaml
    /// </summary>
    public partial class LoadViewPointView : Window
    {
        public LoadViewPointView(LoadViewPointViewModel viewModel)
        {
            DataContext=viewModel;
            InitializeComponent();
        }
    }
}
