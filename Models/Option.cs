namespace SimuladoConcursos.Models
{
    public class Option
    {
        public char Letra { get; set; }
        public string Texto { get; set; } = string.Empty;

        public override string ToString() => $"{Letra}) {Texto}";
    }
}