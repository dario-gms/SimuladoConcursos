using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

namespace SimuladoConcursos.Views
{
    public partial class AddQuestionPage : Page
    {
        public AddQuestionPage()
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

        private void RespostaCorreta_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Permite apenas letras de A-Z
            e.Handled = !e.Text.All(char.IsLetter);
        }

        private void RespostaCorreta_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text.Length > 0)
            {
                // Garante que seja maiúscula
                textBox.Text = textBox.Text.ToUpper();
                textBox.CaretIndex = textBox.Text.Length;
            }
        }
    }
}