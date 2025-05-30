using Autodesk.Revit.DB;

namespace IssueManager.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : HandyControl.Controls.Window
    {
        public LoginView( LoginViewModel loginViewModel)
        {
            InitializeComponent();
            loginViewModel.CloseAction = this.Close; 
            DataContext = loginViewModel;

        }
    }
}
