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
        public ObservableCollection<string> OpcoesLetras { get; } = new ObservableCollection<string>() { "A", "B", "C", "D", "E" };

        private Question? _currentQuestion;
        public Question? CurrentQuestion
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
                OnPropertyChanged(nameof(ShowProximaButton));
                OnPropertyChanged(nameof(ShowFinalizarButton));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool ShowProximaButton => IsSimuladoRunning && CurrentQuestionIndex < Questions.Count - 1;
        public bool ShowFinalizarButton => IsSimuladoRunning && CurrentQuestionIndex >= Questions.Count - 1;

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

        private string _respostaCorreta = string.Empty;
        public string RespostaCorreta
        {
            get => _respostaCorreta;
            set
            {
                if (!string.IsNullOrEmpty(value)
                    && value.Length == 1
                    && char.IsLetter(value[0]))
                {
                    SetProperty(ref _respostaCorreta, value.ToUpper());
                }
                else
                {
                    SetProperty(ref _respostaCorreta, string.Empty);
                }
            }
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

        private TimeSpan _totalTime;
        public TimeSpan TotalTime
        {
            get => _totalTime;
            set => SetProperty(ref _totalTime, value);
        }

        private int _score;
        public int Score
        {
            get => _score;
            set => SetProperty(ref _score, value);
        }

        public ICommand AddQuestionCommand { get; }
        public ICommand StartSimuladoCommand { get; }
        public ICommand NextQuestionCommand { get; }
        public ICommand FinishSimuladoCommand { get; }

        public event EventHandler<RequestNavigationEventArgs>? RequestNavigation;

        public MainViewModel()
        {
            _databaseService = new DatabaseService();
            _textProcessingService = new TextProcessingService();
            _stopwatch = new Stopwatch();
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;

            AddQuestionCommand = new RelayCommand(AddQuestion);
            StartSimuladoCommand = new RelayCommand(async () => await StartSimuladoAsync(), CanStartSimulado);
            NextQuestionCommand = new RelayCommand(NextQuestion, () => CanGoNext());
            FinishSimuladoCommand = new RelayCommand(FinishSimulado, () => IsSimuladoRunning);

            LoadQuestions();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            TempoDecorrido = _stopwatch.Elapsed;
        }

        private bool CanStartSimulado() => !IsLoading && Questions.Count > 0;

        private bool CanGoNext() => IsSimuladoRunning && !IsLoading && CurrentQuestionIndex < Questions.Count - 1;

        private async Task LoadQuestions()
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
                    CurrentQuestion = Questions.FirstOrDefault();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar questões: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("O enunciado é obrigatório!", "Aviso",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(OpcoesText))
                {
                    MessageBox.Show("As opções são obrigatórias!", "Aviso",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(RespostaCorreta) || RespostaCorreta.Length != 1)
                {
                    MessageBox.Show("Digite uma letra válida para a resposta correta!", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var question = _textProcessingService.ProcessPastedText(
                    Enunciado.Trim(),
                    OpcoesText.Trim()
                );

                if (question.Opcoes.Count < 2)
                {
                    MessageBox.Show("Formato inválido das opções! Use:\nA) Opção A\nB) Opção B", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                question.RespostaCorreta = RespostaCorreta.ToUpper();
                question.AreaConhecimento = AreaConhecimento.Trim();

                await _databaseService.AddQuestionAsync(question);

                Questions.Add(question);

                // Reset dos campos
                Enunciado = string.Empty;
                OpcoesText = string.Empty;
                RespostaCorreta = string.Empty;
                AreaConhecimento = string.Empty;

                MessageBox.Show("Questão adicionada com sucesso!", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task StartSimuladoAsync()
        {
            try
            {
                if (IsLoading || Questions.Count == 0) return;

                IsLoading = true;
                IsSimuladoRunning = true;
                CurrentQuestionIndex = 0;
                CurrentQuestion = Questions[CurrentQuestionIndex];
                Respostas.Clear();

                _stopwatch.Restart();
                _timer.Start();

                RequestNavigation?.Invoke(this, new RequestNavigationEventArgs(typeof(SimuladoPage)));
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void NextQuestion()
        {
            if (CurrentQuestionIndex < Questions.Count - 1)
            {
                CurrentQuestionIndex++;
                CurrentQuestion = Questions[CurrentQuestionIndex];
            }
        }

        public void RegistrarResposta(int questionId, char resposta)
        {
            var questao = Questions.FirstOrDefault(q => q.Id == questionId);
            if (questao == null) return;

            var respostaExistente = Respostas.FirstOrDefault(r => r.QuestionId == questionId);
            bool acertou = resposta.ToString().ToUpper() == questao.RespostaCorreta.ToUpper();

            if (respostaExistente != null)
            {
                respostaExistente.Resposta = resposta;
                respostaExistente.Acertou = acertou;
                respostaExistente.TempoGasto = TempoDecorrido;
            }
            else
            {
                Respostas.Add(new RespostaUsuario
                {
                    QuestionId = questionId,
                    Resposta = resposta,
                    Acertou = acertou,
                    TempoGasto = TempoDecorrido
                });
            }
        }

        private void FinishSimulado()
        {
            _timer.Stop();
            _stopwatch.Stop();
            IsSimuladoRunning = false;
            TotalTime = _stopwatch.Elapsed;

            Resultados.Clear();
            int acertos = 0;

            foreach (var resposta in Respostas)
            {
                var questao = Questions.FirstOrDefault(q => q.Id == resposta.QuestionId);
                if (questao == null) continue;

                Resultados.Add(new ResultadoViewModel
                {
                    Enunciado = questao.Enunciado,
                    RespostaUsuario = resposta.Resposta.ToString(),
                    RespostaCorreta = questao.RespostaCorreta,
                    Acertou = resposta.Acertou ?? false, // Corrigido aqui
                    TempoGasto = resposta.TempoGasto.ToString(@"mm\:ss")
                });

                if (resposta.Acertou.HasValue && resposta.Acertou.Value) acertos++; // Corrigido aqui
            }

            Score = Questions.Count > 0 ? (int)((double)acertos / Questions.Count * 1000) : 0;
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
        public required string Enunciado { get; set; }
        public required string RespostaUsuario { get; set; }
        public required string RespostaCorreta { get; set; }
        public bool Acertou { get; set; }
        public required string TempoGasto { get; set; }
    }
}