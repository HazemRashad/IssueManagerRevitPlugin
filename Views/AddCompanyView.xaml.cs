using IssueManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
