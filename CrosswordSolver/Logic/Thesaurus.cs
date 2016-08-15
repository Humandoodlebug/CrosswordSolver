using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NHunspell;

namespace Logic
{
    public class Thesaurus
    {
        private readonly Hunspell _hunspell;
        private readonly MyThes _thes;

        public Thesaurus(string datFilePath, Dictionary dictionary)
        {
            _hunspell = dictionary.HunspellObj;
            _thes = new MyThes(datFilePath);
        }

        public string[] FindSynonyms(string word)
        {
            var res1 = _thes.Lookup(word)?.Meanings?.Select(x => x.Description).ToArray();
            if (res1 != null)
                return res1;

            var stems = _hunspell.Stem(word);
            if (stems == null || stems.Count == 0)
                return null;
            var res2 = new List<string>();
            foreach (var stem in stems)
            {
                foreach (var meanings in _thes.Lookup(stem)?.Meanings?.Select(z => _hunspell.Generate/*Doesn't work!*/(z.Description, word)) ?? new List<List<string>>())
                    res2.AddRange(meanings);
            }
            return res2.ToArray();
        }
    }
}
