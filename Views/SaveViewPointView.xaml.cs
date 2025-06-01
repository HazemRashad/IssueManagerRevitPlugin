using IssueManager.ViewModels;

namespace IssueManager.Views
{
    /// <summary>
    /// Interaction logic for AddIssueView.xaml
    /// </summary>
    public partial class SaveViewPointView : HandyControl.Controls.Window
    {
        public SaveViewPointView(SaveViewPointViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
            SaveViewPointViewModel.CloseAction = this.Close;
            
        }
    }
}
