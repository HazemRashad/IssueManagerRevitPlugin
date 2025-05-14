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
