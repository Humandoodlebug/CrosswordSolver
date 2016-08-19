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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SC.CrosswordSolver.UI.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void NewButton_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void LoadButton_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void QuitButton_Selected(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Hide();
            Environment.Exit(0);
        }
    }
}
