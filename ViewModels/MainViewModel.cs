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
using System.Windows.Threading;

namespace SimuladoConcursos.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly TextProcessingService _textProcessingService;
        private readonly DispatcherTimer _timer;
        private readonly Stopwatch _stopwatch;

        public ObservableCollection<Question> Questions { get; } = new ObservableCollection<Question>();
        public ObservableCollection<RespostaUsuario> Respostas { get; } = new ObservableCollection<RespostaUsuario>();
        public ObservableCollection<ResultadoViewModel> Resultados { get; } = new ObservableCollection<ResultadoViewModel>();

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
                SetProperty(ref _currentQuestionIndex, value);
                CommandManager.InvalidateRequerySuggested();
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
            set
            {
                SetProperty(ref _isLoading, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private TimeSpan _tempoDecorrido;
        public TimeSpan TempoDecorrido
        {
            get => _tempoDecorrido;
            set => SetProperty(ref _tempoDecorrido, value);
        }

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
            _stopwatch = new Stopwatch();
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;

            AddQuestionCommand = new RelayCommand(AddQuestion);
            StartSimuladoCommand = new RelayCommand(async () => await StartSimuladoAsync(), CanStartSimulado);
            NextQuestionCommand = new RelayCommand(NextQuestion, CanGoNext);
            PreviousQuestionCommand = new RelayCommand(PreviousQuestion, CanGoPrevious);
            SubmitAnswerCommand = new RelayCommand(SubmitAnswer);
            FinishSimuladoCommand = new RelayCommand(FinishSimulado, () => IsSimuladoRunning);

            LoadQuestions();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TempoDecorrido = _stopwatch.Elapsed;
        }

        private bool CanStartSimulado() => !IsLoading && Questions.Count > 0;
        private bool CanGoNext() => IsSimuladoRunning && !IsLoading && CurrentQuestionIndex < Questions.Count - 1;
        private bool CanGoPrevious() => IsSimuladoRunning && !IsLoading && CurrentQuestionIndex > 0;

        private async void LoadQuestions()
        {
            try
            {
                IsLoading = true;
                var questions = await _databaseService.GetAllQuestionsAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
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
                Resultados.Clear();
                _stopwatch.Restart();
                _timer.Start();
                TempoDecorrido = TimeSpan.Zero;

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

        private void NextQuestion()
        {
            CurrentQuestionIndex++;
            CurrentQuestion = Questions[CurrentQuestionIndex];
        }

        private void PreviousQuestion()
        {
            CurrentQuestionIndex--;
            CurrentQuestion = Questions[CurrentQuestionIndex];
        }

        private void SubmitAnswer()
        {
            // Implementar lógica de resposta
        }

        public void RegistrarResposta(int questionId, char resposta)
        {
            var respostaExistente = Respostas.FirstOrDefault(r => r.QuestionId == questionId);
            var questao = Questions.First(q => q.Id == questionId);
            bool acertou = resposta == questao.RespostaCorreta;

            if (respostaExistente != null)
            {
                respostaExistente.Resposta = resposta;
                respostaExistente.TempoGasto = TempoDecorrido;
                respostaExistente.Acertou = acertou;
            }
            else
            {
                Respostas.Add(new RespostaUsuario
                {
                    QuestionId = questionId,
                    Resposta = resposta,
                    TempoGasto = TempoDecorrido,
                    Acertou = acertou
                });
            }
        }

        private void FinishSimulado()
        {
            _timer.Stop();
            _stopwatch.Stop();
            IsSimuladoRunning = false;

            Resultados.Clear();
            foreach (var resposta in Respostas)
            {
                var questao = Questions.First(q => q.Id == resposta.QuestionId);
                Resultados.Add(new ResultadoViewModel
                {
                    Enunciado = questao.Enunciado,
                    RespostaUsuario = resposta.Resposta.ToString(),
                    RespostaCorreta = questao.RespostaCorreta.ToString(),
                    Acertou = resposta.Acertou ?? false,
                    TempoGasto = resposta.TempoGasto.ToString(@"mm\:ss")
                });
            }

            RequestNavigation?.Invoke(this, new RequestNavigationEventArgs(typeof(ResultadoPage)));
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

    public class ResultadoViewModel
    {
        public string Enunciado { get; set; }
        public string RespostaUsuario { get; set; }
        public string RespostaCorreta { get; set; }
        public bool Acertou { get; set; }
        public string TempoGasto { get; set; }
    }
}