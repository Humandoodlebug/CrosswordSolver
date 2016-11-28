using System;
using System.Collections.Generic;
using NHunspell;

namespace SC.CrosswordSolver.Logic
{
    public class Dictionary : IDisposable
    {
        public Hunspell HunspellObj { get; }

        public Dictionary(string affixFilePath, string dictionaryFilePath)
        {
            HunspellObj = new Hunspell(affixFilePath, dictionaryFilePath);
        }

        public bool Verify(string word) => HunspellObj.Spell(word);
        public List<string> Stem(string word) => HunspellObj.Stem(word);
        public List<string> Generate(string word, string sample) => HunspellObj.Generate(word, sample);

        ~Dictionary()
        {
            HunspellObj.Dispose();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}