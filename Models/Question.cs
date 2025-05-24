using System;
using System.Collections.Generic;

namespace SimuladoConcursos.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Enunciado { get; set; } = string.Empty;
        public List<Option> Opcoes { get; set; } = new List<Option>();
        public string RespostaCorreta { get; set; } = string.Empty; // Alterado para string
        public string AreaConhecimento { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}