using System.Windows;
using SimuladoConcursos.Views;
using SimuladoConcursos.ViewModels;
using System.Windows.Controls;

namespace SimuladoConcursos
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowWelcomePage();
        }

        public void ShowWelcomePage()
        {
            NavigateToPage(new WelcomePage());
        }

        public void ShowAddQuestionPage()
        {
            NavigateToPage(new AddQuestionPage());
        }

        public void ShowSimuladoPage(SimuladoPage page = null)
        {
            MainFrame.Navigate(page ?? new SimuladoPage());
        }

        public void NavigateToPage(Page page)
        {
            Content = page;

            // Forçar atualização do DataContext se necessário
            if (page.DataContext == null && Content is FrameworkElement element)
            {
                element.DataContext = new MainViewModel();
            }
        }
    }
}