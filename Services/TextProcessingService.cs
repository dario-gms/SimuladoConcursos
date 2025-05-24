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
            // Validação inicial simplificada
            if (string.IsNullOrWhiteSpace(enunciado))
                throw new ArgumentException("O enunciado não pode ser vazio");

            if (string.IsNullOrWhiteSpace(opcoesText))
                throw new ArgumentException("As opções não podem estar vazias");

            var question = new Question
            {
                Enunciado = enunciado.Trim(),
                Opcoes = ParseOptions(opcoesText.Trim())
            };

            // Valida quantidade mínima de opções
            if (question.Opcoes.Count < 2)
                throw new ArgumentException("Deve haver pelo menos 2 opções válidas");

            return question;
        }

        private List<Option> ParseOptions(string opcoesText)
        {
            var options = new List<Option>();

            var lines = opcoesText.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.RemoveEmptyEntries
            );

            foreach (var line in lines)
            {
                if (TryParseOptionLine(line, out var option))
                {
                    options.Add(option);
                }
            }

            return options;
        }

        private bool TryParseOptionLine(string line, out Option option)
        {
            option = null;

            if (string.IsNullOrWhiteSpace(line) || line.Length < 2)
                return false;

            char letra = line[0];
            char separador = line[1];

            if (!char.IsLetter(letra) ||
               (separador != ')' && separador != '.' && separador != ' ' && separador != '-'))
            {
                return false;
            }

            option = new Option
            {
                Letra = char.ToUpper(letra),
                Texto = line.Substring(2).Trim()
            };

            return !string.IsNullOrWhiteSpace(option.Texto);
        }
    }
}