using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using SC.CrosswordSolver.UI.Annotations;
using SC.CrosswordSolver.UI.Model;

namespace SC.CrosswordSolver.UI.ViewModels
{
    public class CellViewModel : INotifyPropertyChanged
    {
        public enum CellState { Active, Starred, Inactive }

        private CellState _isEnabled;
        private char? _character;
        public static  SolidColorBrush InactiveColour => Brushes.Black;
        public static SolidColorBrush ActiveColour => Brushes.White;
        public static SolidColorBrush StarredColour => Brushes.CornflowerBlue;
        public ICommand ButtonClickCommand => new DelegateCommand(obj => ButtonClick());
        [NotNull] public MainViewModel ParentModel { get; set; }

        public double CellOpacity
        {
            get
            {
                if (IsEnabled == CellState.Inactive)
                    return 0;
                return 1;
            }
        }

        public void ButtonClick()
        {
            switch (ParentModel.LayoutGridMode)
            {
                case LayoutInteractionMode.InvertActive:
                    IsEnabled = IsEnabled == CellState.Inactive ? CellState.Active : CellState.Inactive;
                    break;
                case LayoutInteractionMode.InvertStarred:
                    switch (IsEnabled)
                    {
                        case CellState.Active:
                            IsEnabled = CellState.Starred;
                            break;
                        case CellState.Starred:
                            IsEnabled = CellState.Active;
                            break;
                    }
                    break;
            }
        }

        public SolidColorBrush CellBackground
        {
            get
            {
                switch (IsEnabled)
                {
                    case CellState.Active:
                        return ActiveColour;
                    case CellState.Inactive:
                        return InactiveColour;
                    case CellState.Starred:
                        return StarredColour;
                    default: throw new InvalidEnumArgumentException();
                }
            }
        }


        public CellState IsEnabled
        {
            get { return _isEnabled; }

            set
            {
                if (value == _isEnabled) return;
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
                OnPropertyChanged(nameof(CellBackground));
            }
        }

        public char? Character
        {
            get { return _character; }

            set
            {
                if (value.ToString().ToUpper()[0] == _character) return;
                _character = value.ToString().ToUpper()[0];
                OnPropertyChanged(nameof(Character));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
