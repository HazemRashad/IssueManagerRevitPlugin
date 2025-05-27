using IssueManager.ViewModels;

namespace IssueManager.Views
{
    /// <summary>
    /// Interaction logic for AddIssueView.xaml
    /// </summary>
    public partial class AddIssueView : HandyControl.Controls.Window
    {
        public AddIssueView(AddIssueViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
