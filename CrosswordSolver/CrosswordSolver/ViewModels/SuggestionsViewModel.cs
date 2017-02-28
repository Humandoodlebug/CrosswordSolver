using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SC.CrosswordSolver.Logic;
using SC.CrosswordSolver.UI.Annotations;

namespace SC.CrosswordSolver.UI.ViewModels
{
    public class SuggestionsViewModel : INotifyPropertyChanged
    {
        private readonly Dictionary _dictionary;
        private readonly Lookup _lookup;
        private string _currentWord = string.Empty;
        private string _searchString; //TODO: Link the search string up to filter the results
        private string[] _suggestions = new string[0];
        private Thesaurus _thesaurus;
        private Task _currentGetSuggestionsTask;
        private CancellationToken _cancellationToken;
        private CancellationTokenSource _tokenSource;

        public SuggestionsViewModel(bool ofNoConsequence) //For debugging and Designer work.
        {
            Suggestions = new[] {"TestString", "TestString2", "TestString3..."};
        }

        public SuggestionsViewModel(Lookup lookup)
        {
            _lookup = lookup;
        }

        public SuggestionsViewModel(Dictionary dictionary, Thesaurus thesaurus)
        {
            _dictionary = dictionary;
            _thesaurus = thesaurus;
        }

        public SuggestionsViewModel()
        {
            //_dictionary = new Dictionary("Dictionaries/en_GB.aff", "Dictionaries/en_GB.dic");
            //_thesaurus = new Thesaurus("Thesauruses/th_en_US_v2.dat", _dictionary);
            _lookup = new Lookup("Dictionaries/corncob_caps_WordList.txt");
        }

        public string[] Suggestions
        {
            get { return _suggestions; }
            set
            {
                if (value == _suggestions) return;
                _suggestions = value;
                OnPropertyChanged();
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
                Suggest(_currentWord.ToUpper());
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void Suggest(string word)
        {


            _tokenSource = new CancellationTokenSource();
            _cancellationToken = _tokenSource.Token;

            _currentGetSuggestionsTask = Task.Run(() =>
            {
                _currentWord = word;
                string[] suggestionsArray;
                if (string.IsNullOrEmpty(SearchString))
                    suggestionsArray = _lookup.Suggest(word).Select(x => x.ToLower()).ToArray();
                else
                {
                    var searchString = SearchString.ToUpper();
                    var suggestions = _lookup.Suggest(word);
                    suggestionsArray =
                        suggestions.Where(x => x.StartsWith(searchString)).Select(x => x.ToLower()).ToArray();
                    _cancellationToken.ThrowIfCancellationRequested();
                    var secondarySuggestionsArray =
                        suggestions.Where(x => !x.StartsWith(searchString) && x.Contains(searchString)).ToArray();
                    _cancellationToken.ThrowIfCancellationRequested();
                    if (suggestionsArray.Length + secondarySuggestionsArray.Length <= 1000)
                    {
                        suggestionsArray =
                            suggestionsArray.Concat(suggestions.Where(x => x.Contains(searchString))).ToArray();
                        _cancellationToken.ThrowIfCancellationRequested();
                    }
                }
                Suggestions = suggestionsArray.Length > 1000 ? new string[0] : suggestionsArray;
            }, _cancellationToken);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}