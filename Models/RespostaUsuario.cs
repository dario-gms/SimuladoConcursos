namespace SimuladoConcursos.Models
{
    public class RespostaUsuario
    {
        public int QuestionId { get; set; }
        public char Resposta { get; set; }
        public bool? Acertou { get; set; } = null; // Mantém como nullable
        public TimeSpan TempoGasto { get; set; }
    }
}