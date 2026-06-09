using System;
using System.Collections.Generic;

namespace EmailDispatcher.Application.Services.Utils
{
    public static class HomoglyphGenerator
    {
        private static readonly Random _random = new();

        private static readonly Dictionary<char, char[]> Replacements =
            new()
            {
                ['A'] = new[] { '4', 'А' },
                ['B'] = new[] { '8' },
                ['E'] = new[] { '3', 'Е' },
                ['I'] = new[] { '1', 'І' },
                ['L'] = new[] { '1' },
                ['O'] = new[] { '0', 'О' },
                ['S'] = new[] { '5', '$' },
                ['T'] = new[] { '7', 'Τ' },
                ['C'] = new[] { 'С' },
                ['P'] = new[] { 'Р' },
                ['X'] = new[] { 'Χ' },
                ['H'] = new[] { 'Н' }
            };
        public static string Transform(string text)
        {
            var chars = text.ToCharArray();

            var candidatos = new List<int>();

            for (int i = 0; i < chars.Length; i++)
            {
                char upper = char.ToUpperInvariant(chars[i]);

                if (Replacements.ContainsKey(upper))
                {
                    candidatos.Add(i);
                }
            }

            if (candidatos.Count == 0)
                return text;

            int quantidadeTrocas =
                Math.Min(
                    _random.Next(1, 3), // 1 ou 2 trocas
                    candidatos.Count
                );

            for (int t = 0; t < quantidadeTrocas; t++)
            {
                int sorteado =
                    _random.Next(candidatos.Count);

                int posicao =
                    candidatos[sorteado];

                candidatos.RemoveAt(sorteado);

                char upper =
                    char.ToUpperInvariant(chars[posicao]);

                var options =
                    Replacements[upper];

                chars[posicao] =
                    options[_random.Next(options.Length)];
            }

            return new string(chars);
        }
    }
}