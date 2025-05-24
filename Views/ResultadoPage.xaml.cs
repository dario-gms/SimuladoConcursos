using System.Windows;
using System.Windows.Controls;

namespace SimuladoConcursos.Views
{
    public partial class ResultadoPage : Page
    {
        public ResultadoPage()
        {
            InitializeComponent();
        }

        private void VoltarButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.ShowWelcomePage();
            }
        }
    }
}