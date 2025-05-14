using IssueManager.ViewModels;

namespace IssueManager.Views
{
    public sealed partial class SaveViewPointView
    {
        public SaveViewPointView(SaveViewPointViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}