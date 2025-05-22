namespace SimuladoConcursos.Models
{
    public class RespostaUsuario
    {
        public int QuestionId { get; set; }  // ID da questão respondida
        public char Resposta { get; set; }   // Letra da resposta (A, B, C, etc.)
        public TimeSpan TempoGasto { get; set; } // Tempo gasto na questão

        // Indica se a resposta foi correta (será preenchido posteriormente)
        public bool? Acertou { get; set; } = null;

        // Método para exibição formatada
        public string RespostaFormatada => $"{Resposta} (Tempo: {TempoGasto:mm\\:ss})";
    }
}