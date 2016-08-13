using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHunspell;

namespace Logic
{
    public class Dictionary
    {
        private readonly Hunspell _hunspell = new Hunspell("en_gb.aff","en_gb.dic");

        public bool Verify(string word) => _hunspell.Spell(word);
        public List<string> Stem(string word) => _hunspell.Stem(word);

        ~Dictionary()
        {
            _hunspell.Dispose();
        }
    }
}
