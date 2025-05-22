using SimuladoConcursos.Models;
using SimuladoConcursos.Services;
using SimuladoConcursos.Utilities;
using SimuladoConcursos.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
            set => SetProperty(ref _currentQuestionIndex, value);
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
            set
            {
                SetProperty(ref _isLoading, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private Stopwatch _stopwatch = new Stopwatch();
        public TimeSpan TempoDecorrido => _stopwatch.Elapsed;

        public ICommand AddQuestionCommand { get; }
        public ICommand StartSimuladoCommand { get; }
        public ICommand NextQuestionCommand { get; }
        public ICommand PreviousQuestionCommand { get; }
        public ICommand SubmitAnswerCommand { get; }
        public ICommand FinishSimuladoCommand { get; }

        public event EventHandler<RequestNavigationEventArgs> RequestNavigation;

        public MainViewModel()
        {
            _databaseService = new DatabaseService();
            _textProcessingService = new TextProcessingService();

            AddQuestionCommand = new RelayCommand(AddQuestion);
            StartSimuladoCommand = new RelayCommand(async () => await StartSimuladoAsync(), CanStartSimulado);
            NextQuestionCommand = new RelayCommand(NextQuestion, CanNavigate);
            PreviousQuestionCommand = new RelayCommand(PreviousQuestion, CanNavigate);
            SubmitAnswerCommand = new RelayCommand(SubmitAnswer);
            FinishSimuladoCommand = new RelayCommand(FinishSimulado);

            LoadQuestions();
        }

        private bool CanStartSimulado() => !IsLoading && Questions.Count > 0;

        private async void LoadQuestions()
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
                    CommandManager.InvalidateRequerySuggested();
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
                    Enunciado = string.Empty;
                    OpcoesText = string.Empty;
                    RespostaCorreta = '\0';
                    AreaConhecimento = string.Empty;
                    CommandManager.InvalidateRequerySuggested();
                });

                MessageBox.Show("Questão adicionada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
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
                if (IsLoading) return;

                IsLoading = true;
                IsSimuladoRunning = true;
                CurrentQuestionIndex = 0;
                CurrentQuestion = Questions[CurrentQuestionIndex];
                Respostas.Clear();
                _stopwatch.Restart();

                RequestNavigation?.Invoke(this, new RequestNavigationEventArgs(typeof(SimuladoPage)));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao iniciar simulado: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                respostaExistente.Acertou = resposta == Questions.First(q => q.Id == questionId).RespostaCorreta;
            }
            else
            {
                Respostas.Add(new RespostaUsuario
                {
                    QuestionId = questionId,
                    Resposta = resposta,
                    TempoGasto = TempoDecorrido,
                    Acertou = resposta == Questions.First(q => q.Id == questionId).RespostaCorreta
                });
            }
        }

        private void FinishSimulado()
        {
            _stopwatch.Stop();
            IsSimuladoRunning = false;
            RequestNavigation?.Invoke(this, new RequestNavigationEventArgs(typeof(WelcomePage)));
        }
    }

    public class RequestNavigationEventArgs : EventArgs
    {
        public Type TargetPageType { get; }

        public RequestNavigationEventArgs(Type targetPageType)
        {
            TargetPageType = targetPageType;
        }
    }
}