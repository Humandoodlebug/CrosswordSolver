using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SC.CrosswordSolver.UI.Annotations;
using SC.CrosswordSolver.UI.Model;

namespace SC.CrosswordSolver.UI.ViewModels
{
    public class CellViewModel : INotifyPropertyChanged
    {
        public enum CellSelectedState
        {
            Selected,
            WordSelected,
            NotSelected
        }

        public enum CellState
        {
            Active,
            Starred,
            Inactive
        }

        public CellViewModel(MainViewModel parentModel)
        {
            ParentModel = parentModel;
        }

        private char? _character;

        private CellState _isEnabled;
        private CellSelectedState _selectionState = CellSelectedState.NotSelected;
        private int _buttonBorderThickness;

        public static SolidColorBrush InactiveBackgroundColour => Brushes.Black;
        public static SolidColorBrush ActiveBackgroundColour => Brushes.White;
        public static SolidColorBrush StarredBackgroundColour => Brushes.CornflowerBlue;
        public ICommand ButtonClickCommand => new DelegateCommand(obj => ButtonClick());

        public int ButtonBorderThickness
        {
            get { return _buttonBorderThickness; }
            set
            {
                if (value == _buttonBorderThickness) return;
                _buttonBorderThickness = value;
                OnPropertyChanged(nameof(ButtonBorderThickness));
            }
        }

        [NotNull]
        public MainViewModel ParentModel { get; set; }

        private CellSelectedState VisualSelectionState
        {
            set
            {
                switch (value)
                {
                    case CellSelectedState.Selected:
                        ButtonBorderThickness = 2;
                        break;
                    case CellSelectedState.WordSelected:
                        ButtonBorderThickness = 1;
                        break;
                    case CellSelectedState.NotSelected:
                        ButtonBorderThickness = 0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        public void SelectionDirectionValidator(ref WordDirection direction, int row, int column)
        {
            if (direction == WordDirection.Across && (column + 1 >= ParentModel.Width || ParentModel.CrosswordData[row][column + 1].IsEnabled == CellState.Inactive) && (column - 1 < 0 || ParentModel.CrosswordData[row][column-1].IsEnabled == CellState.Inactive))
                direction = WordDirection.Down;
            else if (direction == WordDirection.Down && (row + 1 >= ParentModel.Height || ParentModel.CrosswordData[row + 1][column].IsEnabled == CellState.Inactive) && (row - 1 < 0 || ParentModel.CrosswordData[row-1][column].IsEnabled == CellState.Inactive))
                direction = WordDirection.Across;
        }

        public CellSelectedState SelectionState
        {
            get { return _selectionState; }
            set
            {
                if (IsEnabled == CellState.Inactive)
                {
                    ParentModel.SelectedRow = -1;
                    return;
                }
                if ((value != CellSelectedState.WordSelected) || (_selectionState != CellSelectedState.Selected))
                {
                    _selectionState = value;
                    VisualSelectionState = value;
                    OnPropertyChanged(nameof(SelectionState));
                }

                int row, column;
                GetPosition(out row, out column);
                if (value == CellSelectedState.Selected)
                {
                    ParentModel.SelectedRow = row;
                    ParentModel.SelectedColumn = column;
                    GetWordStart(ref row, ref column, ParentModel.SelectionDirection);
                    value = CellSelectedState.WordSelected;
                    ParentModel.SelectedWordRow = row;
                    ParentModel.SelectedWordColumn = column;
                    if (ParentModel.SelectionDirection == WordDirection.Down)
                        row--;
                    else column--;
                }

                switch (ParentModel.SelectionDirection)
                {
                    case WordDirection.Down:
                        if ((row + 1 < ParentModel.CrosswordData.Count) &&
                            (ParentModel.CrosswordData[row + 1][column].IsEnabled != CellState.Inactive))
                            ParentModel.CrosswordData[row + 1][column].SelectionState = value;
                        break;
                    case WordDirection.Across:
                        if ((column + 1 < ParentModel.CrosswordData[row].Count) &&
                            (ParentModel.CrosswordData[row][column + 1].IsEnabled != CellState.Inactive))
                            ParentModel.CrosswordData[row][column + 1].SelectionState = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public double CellOpacity => IsEnabled == CellState.Inactive ? 0 : 1;


        public SolidColorBrush CellBackground
        {
            get
            {
                switch (IsEnabled)
                {
                    case CellState.Active:
                        return ActiveBackgroundColour;
                    case CellState.Inactive:
                        return InactiveBackgroundColour;
                    case CellState.Starred:
                        return StarredBackgroundColour;
                    default:
                        throw new InvalidEnumArgumentException();
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
                if (value == null)
                {
                    _character = null;
                    OnPropertyChanged(nameof(Character));
                    return;
                }
                if (value.ToString().ToUpper()[0] == _character) return;
                _character = value.ToString().ToUpper()[0];
                OnPropertyChanged(nameof(Character));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void GetWordStart(ref int row, ref int column, WordDirection orientation)
        {
            if (orientation == WordDirection.Across)
                while ((column > 0) && (ParentModel.CrosswordData[row][column - 1].IsEnabled != CellState.Inactive))
                    column--;

            else if (orientation == WordDirection.Down)
                while ((row > 0) && (ParentModel.CrosswordData[row - 1][column].IsEnabled != CellState.Inactive))
                    row--;
            else throw new InvalidEnumArgumentException(nameof(orientation), (int)orientation, typeof(WordDirection));
        }

        private void GetPosition(out int i, out int j)
        {
            j = -1;
            var found = false;
            for (i = 0; i < ParentModel.CrosswordData.Count; i++)
            {
                for (j = 0; j < ParentModel.CrosswordData[i].Count; j++)
                    if (ParentModel.CrosswordData[i][j] == this)
                    {
                        found = true;
                        break;
                    }
                if (found) break;
            }
            if (j == -1) throw new Exception("The ViewModel Crossword Collection is empty!");
        }

        public void ButtonClick()
        {
            if (ParentModel.IsLayoutModeActive)
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
            else
            {

                if (SelectionState == CellSelectedState.Selected)
                {
                    ParentModel.CrosswordData[ParentModel.SelectedWordRow][ParentModel.SelectedWordColumn].SelectionState = CellSelectedState.NotSelected;

                    ParentModel.SelectionDirection = ParentModel.SelectionDirection == WordDirection.Across
                        ? WordDirection.Down
                        : WordDirection.Across;
                }
                else if (ParentModel.SelectedWordRow != -1)
                    ParentModel.CrosswordData[ParentModel.SelectedWordRow][ParentModel.SelectedWordColumn].SelectionState = CellSelectedState.NotSelected;

                int row, column;
                GetPosition(out row, out column);
                var sd = ParentModel.SelectionDirection;
                SelectionDirectionValidator(ref sd, row, column);
                ParentModel.SelectionDirection = sd;

                SelectionState = CellSelectedState.Selected;
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}