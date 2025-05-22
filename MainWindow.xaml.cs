using SimuladoConcursos.ViewModels;
using SimuladoConcursos.Views;
using System.Windows;

namespace SimuladoConcursos
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            _viewModel.RequestNavigation += OnRequestNavigation;

            ShowWelcomePage();
        }

        private void OnRequestNavigation(object sender, ViewModels.RequestNavigationEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.TargetPageType == typeof(SimuladoPage))
                {
                    var simuladoPage = new SimuladoPage();
                    simuladoPage.DataContext = _viewModel;
                    MainFrame.Navigate(simuladoPage);
                }
                else if (e.TargetPageType == typeof(WelcomePage))
                {
                    var welcomePage = new WelcomePage();
                    welcomePage.DataContext = _viewModel;
                    MainFrame.Navigate(welcomePage);
                }
                else if (e.TargetPageType == typeof(AddQuestionPage))
                {
                    var addQuestionPage = new AddQuestionPage();
                    addQuestionPage.DataContext = _viewModel;
                    MainFrame.Navigate(addQuestionPage);
                }
            });
        }

        public void ShowWelcomePage()
        {
            var welcomePage = new WelcomePage();
            welcomePage.DataContext = _viewModel;
            MainFrame.Navigate(welcomePage);
        }

        public void ShowAddQuestionPage()
        {
            var addQuestionPage = new AddQuestionPage();
            addQuestionPage.DataContext = _viewModel;
            MainFrame.Navigate(addQuestionPage);
        }

        public void ShowSimuladoPage()
        {
            var simuladoPage = new SimuladoPage();
            simuladoPage.DataContext = _viewModel;
            MainFrame.Navigate(simuladoPage);
        }
    }
}