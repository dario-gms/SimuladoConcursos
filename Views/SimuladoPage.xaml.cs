using System.Windows;
using System.Windows.Controls;
using SimuladoConcursos.ViewModels;
using System.Threading.Tasks;

namespace SimuladoConcursos.Views
{
    public partial class SimuladoPage : Page
    {
        public SimuladoPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                if (!viewModel.IsSimuladoRunning)
                {
                    await viewModel.StartSimuladoAsync();
                }
            }
        }

        private void Option_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.RadioButton radioButton &&
                radioButton.Tag is char letra &&
                DataContext is MainViewModel viewModel)
            {
                viewModel.RegistrarResposta(viewModel.CurrentQuestion.Id, letra);
            }
        }
    }
}