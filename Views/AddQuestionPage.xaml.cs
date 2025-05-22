using System.Windows;
using System.Windows.Controls;

namespace SimuladoConcursos.Views
{
    public partial class AddQuestionPage : Page
    {
        public AddQuestionPage()
        {
            InitializeComponent();

            // Garante que o DataContext seja criado se não foi definido no XAML
            if (DataContext == null)
            {
                DataContext = new ViewModels.MainViewModel();
            }
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