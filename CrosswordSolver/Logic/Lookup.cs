using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC.CrosswordSolver.Logic
{
    public class Lookup
    {
        private readonly string[] _words;
        public Lookup(string wordListPath)
        {
            _words = File.ReadAllLines(wordListPath);
        }

        public string[] Suggest(string word)
        {
            var possibleWords = _words.Where(x => x.Length == word.Length).ToArray();
            for (int i = 0; i < word.Length; i++)
                if (word[i] != ' ')
                    possibleWords = possibleWords.Where(x => x[i] == word[i]).ToArray();
            return possibleWords;
        }

        private static bool IsValidSuggestion(string word, string suggestion)
        {
            if (word.Length != suggestion.Length) return false;
            return !word.Where((c, i) => c != '*' && c != suggestion[i]).Any();
        }
    }
}
