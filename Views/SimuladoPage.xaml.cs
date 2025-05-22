using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SimuladoConcursos.ViewModels;
using System.Threading.Tasks;
using SimuladoConcursos.Models;

namespace SimuladoConcursos.Views
{
    public partial class SimuladoPage : Page
    {
        public SimuladoPage()
        {
            InitializeComponent();
            Loaded += SimuladoPage_Loaded;
        }

        private async void SimuladoPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                try
                {
                    await viewModel.StartSimuladoAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar simulado: {ex.Message}",
                                  "Erro",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);

                    if (Application.Current.MainWindow is MainWindow mainWindow)
                    {
                        mainWindow.ShowWelcomePage();
                    }
                }
            }
        }

        private void Option_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton &&
                radioButton.Tag is char letra &&
                DataContext is MainViewModel viewModel)
            {
                var resposta = new RespostaUsuario
                {
                    QuestionId = viewModel.CurrentQuestion.Id,
                    Resposta = letra,
                    TempoGasto = viewModel.TempoDecorrido
                };

                // Aqui você pode adicionar a resposta à coleção
                // ou chamar um método no ViewModel para registrar
            }
        }
    }
}