using System.ComponentModel;
using System.Runtime.CompilerServices;
using SC.CrosswordSolver.Logic;
using SC.CrosswordSolver.UI.Annotations;

namespace SC.CrosswordSolver.UI.ViewModels
{
    public class SuggestionsViewModel : INotifyPropertyChanged
    {
        private readonly Dictionary _dictionary;
        private readonly Lookup _lookup;
        private string _searchString; //TODO: Link the search string up to filter the results
        private string[] _suggestions;
        private Thesaurus _thesaurus;

        public SuggestionsViewModel(Dictionary dictionary, Thesaurus thesaurus, Lookup lookup)
        {
            _dictionary = dictionary;
            _thesaurus = thesaurus;
            _lookup = lookup;
        }

        public SuggestionsViewModel()
        {
            _dictionary = new Dictionary("Dictionaries/en_GB.aff", "Dictionaries/en_GB.dic");
            _thesaurus = new Thesaurus("Thesauruses/th_en_US_v2.dat", _dictionary);
            _lookup = new Lookup("Dictionaries/corncob_caps_WordList.txt");
        }

        public string[] Suggestions
        {
            get { return _suggestions; }
            set
            {
                if (Equals(value, _suggestions)) return;
                _suggestions = value;
                OnPropertyChanged(nameof(Suggestions));
            }
        }

        public string SearchString
        {
            get { return _searchString; }
            set
            {
                if (value == _searchString) return;
                _searchString = value;

                OnPropertyChanged(nameof(SearchString));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void Suggest(string word) => Suggestions = _lookup.Suggest(word);

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}