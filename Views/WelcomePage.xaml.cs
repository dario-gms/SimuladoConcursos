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

        private void NavigateToSimulado(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                // Cria uma nova instância da página para evitar problemas de estado
                mainWindow.ShowSimuladoPage(new SimuladoPage());
            }
        }
    }
}