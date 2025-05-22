using SimuladoConcursos.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimuladoConcursos.Services
{
    public class TextProcessingService
    {
        public Question ProcessPastedText(string enunciado, string opcoesText)
        {
            // Verifica se os parâmetros são válidos
            if (string.IsNullOrWhiteSpace(enunciado))
                throw new ArgumentException("O enunciado não pode ser vazio");

            if (string.IsNullOrWhiteSpace(opcoesText))
                throw new ArgumentException("As opções não podem estar vazias");

            var question = new Question
            {
                Enunciado = enunciado,
                Opcoes = ParseOptions(opcoesText)
            };

            return question;
        }

        private List<Option> ParseOptions(string opcoesText)
        {
            var options = new List<Option>();

            if (string.IsNullOrWhiteSpace(opcoesText))
                return options;

            var lines = opcoesText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (line.Length >= 2 && char.IsLetter(line[0]) && line[1] == ' ')
                {
                    options.Add(new Option
                    {
                        Letra = char.ToUpper(line[0]),
                        Texto = line.Substring(2).Trim()
                    });
                }
            }

            return options;
        }
    }
}