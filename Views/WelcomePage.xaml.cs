using System.Windows;
using System.Windows.Controls;

namespace SimuladoConcursos.Views
{
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void NavigateToAddQuestionPage(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.ShowAddQuestionPage();
            }
        }
    }
}