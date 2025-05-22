using System.Windows;
using System.Windows.Controls;
using SimuladoConcursos.ViewModels;

namespace SimuladoConcursos.Views
{
    public partial class ResultadoPage : Page
    {
        public ResultadoPage()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void VoltarButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new WelcomePage());
        }
    }
}