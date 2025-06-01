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
    /// Interaction logic for RevitAddinView.xaml
    /// </summary>
    public partial class RevitAddinView : Window
    {
        public RevitAddinView( RevitAddInViewModel revitAddInViewModel)
        {
            InitializeComponent();
            DataContext = revitAddInViewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
