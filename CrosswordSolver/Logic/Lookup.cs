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
        private string[] _words;
        public Lookup(string wordListPath)
        {
            _words = File.ReadAllLines(wordListPath);
        }

        public string[] Suggest(string word) => _words.Where((suggestion) => IsValidSuggestion(word, suggestion)).ToArray();

        private static bool IsValidSuggestion(string word, string suggestion)
        {
            if (word.Length != suggestion.Length) return false;
            return !word.Where((c, i) => c != '*' && c != suggestion[i]).Any();
        }
    }
}
