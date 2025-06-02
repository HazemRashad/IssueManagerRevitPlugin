using HandyControl.Controls;
using IssueManager.ViewModels;

namespace IssueManager.Views
{
    public partial class LoadViewPointView : HandyControl.Controls.Window
    {
        public LoadViewPointView(LoadViewPointViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}