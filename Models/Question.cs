using System;
using System.Collections.Generic;

namespace SimuladoConcursos.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Enunciado { get; set; }
        public List<Option> Opcoes { get; set; } = new List<Option>();
        public char RespostaCorreta { get; set; }
        public string AreaConhecimento { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}