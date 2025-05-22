using SimuladoConcursos.Models;
using SimuladoConcursos.Services;
using SimuladoConcursos.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SimuladoConcursos.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly TextProcessingService _textProcessingService;

        public ObservableCollection<Question> Questions { get; } = new ObservableCollection<Question>();
        public ObservableCollection<RespostaUsuario> Respostas { get; } = new ObservableCollection<RespostaUsuario>();

        private Question _currentQuestion;
        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set => SetProperty(ref _currentQuestion, value);
        }

        private int _currentQuestionIndex;
        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set
            {
                _currentQuestionIndex = value;
                ((RelayCommand)NextQuestionCommand).RaiseCanExecuteChanged();
                ((RelayCommand)PreviousQuestionCommand).RaiseCanExecuteChanged();
            }
        }

        private string _enunciado = string.Empty;
        public string Enunciado
        {
            get => _enunciado;
            set => SetProperty(ref _enunciado, value);
        }

        private string _opcoesText = string.Empty;
        public string OpcoesText
        {
            get => _opcoesText;
            set => SetProperty(ref _opcoesText, value);
        }

        private char _respostaCorreta;
        public char RespostaCorreta
        {
            get => _respostaCorreta;
            set => SetProperty(ref _respostaCorreta, value);
        }

        private string _areaConhecimento = string.Empty;
        public string AreaConhecimento
        {
            get => _areaConhecimento;
            set => SetProperty(ref _areaConhecimento, value);
        }

        private bool _isSimuladoRunning;
        public bool IsSimuladoRunning
        {
            get => _isSimuladoRunning;
            set => SetProperty(ref _isSimuladoRunning, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private Stopwatch _stopwatch = new Stopwatch();
        public TimeSpan TempoDecorrido => _stopwatch.Elapsed;

        public ICommand AddQuestionCommand { get; }
        public ICommand StartSimuladoCommand { get; }
        public ICommand NextQuestionCommand { get; }
        public ICommand PreviousQuestionCommand { get; }
        public ICommand SubmitAnswerCommand { get; }
        public ICommand FinishSimuladoCommand { get; }

        public MainViewModel()
        {
            _databaseService = new DatabaseService();
            _textProcessingService = new TextProcessingService();

            // Carrega as questões de forma assíncrona ao iniciar
            Task.Run(() => LoadQuestionsAsync());

            AddQuestionCommand = new RelayCommand(AddQuestion);
            StartSimuladoCommand = new RelayCommand(async () => await StartSimuladoAsync());
            NextQuestionCommand = new RelayCommand(NextQuestion, CanNavigate);
            PreviousQuestionCommand = new RelayCommand(PreviousQuestion, CanNavigate);
            SubmitAnswerCommand = new RelayCommand(SubmitAnswer);
            FinishSimuladoCommand = new RelayCommand(FinishSimulado);
        }

        private async Task LoadQuestionsAsync()
        {
            try
            {
                IsLoading = true;
                var questions = await _databaseService.GetAllQuestionsAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Questions.Clear();
                    foreach (var question in questions)
                    {
                        Questions.Add(question);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar questões: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void AddQuestion()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Enunciado))
                {
                    MessageBox.Show("O enunciado é obrigatório!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(OpcoesText))
                {
                    MessageBox.Show("As opções são obrigatórias!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var question = _textProcessingService.ProcessPastedText(
                    Enunciado.Trim(),
                    OpcoesText.Trim()
                );

                if (question.Opcoes.Count == 0)
                {
                    MessageBox.Show("Nenhuma opção válida foi identificada. Formato esperado:\nA) Opção A\nB) Opção B",
                                  "Formato Inválido",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }

                question.RespostaCorreta = char.ToUpper(RespostaCorreta);
                question.AreaConhecimento = AreaConhecimento?.Trim();

                await _databaseService.AddQuestionAsync(question);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Questions.Add(question);

                    // Limpar campos
                    Enunciado = string.Empty;
                    OpcoesText = string.Empty;
                    RespostaCorreta = '\0';
                    AreaConhecimento = string.Empty;

                    MessageBox.Show("Questão adicionada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar questão: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task StartSimuladoAsync()
        {
            try
            {
                if (IsLoading) return; // Previne reentrância

                IsLoading = true;
                await Task.Delay(1); // Permite que a UI atualize o estado de carregamento

                var questions = await _databaseService.GetAllQuestionsAsync();
                Questions.Clear();

                if (questions.Count == 0)
                {
                    MessageBox.Show("Nenhuma questão disponível. Adicione questões primeiro.",
                                  "Aviso",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }

                foreach (var question in questions)
                {
                    Questions.Add(question);
                }

                IsSimuladoRunning = true;
                CurrentQuestionIndex = 0;
                CurrentQuestion = Questions[CurrentQuestionIndex];
                Respostas.Clear();
                _stopwatch.Restart();

                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.ShowSimuladoPage();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao iniciar simulado: {ex.Message}",
                               "Erro",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanNavigate() => IsSimuladoRunning && !IsLoading;

        private void NextQuestion()
        {
            if (CurrentQuestionIndex < Questions.Count - 1)
            {
                CurrentQuestionIndex++;
                CurrentQuestion = Questions[CurrentQuestionIndex];
            }
        }

        private void PreviousQuestion()
        {
            if (CurrentQuestionIndex > 0)
            {
                CurrentQuestionIndex--;
                CurrentQuestion = Questions[CurrentQuestionIndex];
            }
        }

        private void SubmitAnswer()
        {
            // Implementar lógica de resposta
        }

        public void RegistrarResposta(int questionId, char resposta)
        {
            var respostaExistente = Respostas.FirstOrDefault(r => r.QuestionId == questionId);

            if (respostaExistente != null)
            {
                respostaExistente.Resposta = resposta;
                respostaExistente.TempoGasto = TempoDecorrido;
            }
            else
            {
                Respostas.Add(new RespostaUsuario
                {
                    QuestionId = questionId,
                    Resposta = resposta,
                    TempoGasto = TempoDecorrido
                });
            }
        }

        private void FinishSimulado()
        {
            _stopwatch.Stop();
            IsSimuladoRunning = false;
            MessageBox.Show($"Simulado finalizado! Tempo total: {TempoDecorrido:hh\\:mm\\:ss}",
                           "Resultado",
                           MessageBoxButton.OK,
                           MessageBoxImage.Information);

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.ShowWelcomePage();
            }
        }
    }
}