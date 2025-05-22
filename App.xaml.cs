using System.Windows;
using SimuladoConcursos.Data;

namespace SimuladoConcursos
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Garantir que o banco de dados está criado
            try
            {
                AppDbContext.InitializeDatabase();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erro ao inicializar o banco de dados: {ex.Message}",
                              "Erro",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}