using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SC.CrosswordSolver.Logic;
using SC.CrosswordSolver.UI.Annotations;
using SC.CrosswordSolver.UI.Model;
using SC.CrosswordSolver.UI.Views;

namespace SC.CrosswordSolver.UI.ViewModels
{
    public enum WordDirection
    {
        Down,
        Across
    }

    public sealed class MainViewModel : INotifyPropertyChanged
    {
        private Crossword _crossword;
        private ObservableCollection<ObservableCollection<CellViewModel>> _crosswordData;


        private bool _isCrosswordVisible;
        private bool _isDimensionsVisible;
        private bool _isLayoutModeActive;
        private bool _isMenuVisible = true;

        private NavigationState _previousState;
        public int SelectedColumn;
        public int SelectedRow = -1;
        public int SelectedWordColumn;
        public int SelectedWordRow = -1;

        public MainViewModel()
        {
            MenuItems = new ObservableCollection<MenuOptions>
            {
                MenuOptions.NewCrossword,
                MenuOptions.LoadCrossword,
                MenuOptions.Quit
            };
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                SuggestionsDataModel = new SuggestionsViewModel(false);
                return;
            }
#endif
            SuggestionsDataModel = new SuggestionsViewModel();
            //Populate(); //Method for testing purposes
        }

        public string SelectedWord
        {
            get
            {
                if (SelectedWordRow == -1) return null;
                var word = string.Empty;
                var i = 0;
                switch (SelectionDirection)
                {
                    case WordDirection.Down:
                    {
                        while ((SelectedWordRow + i < _crossword.Height) &&
                               (CrosswordData[SelectedWordRow + i][SelectedWordColumn].State !=
                                CellViewModel.CellState.Inactive))
                        {
                            word += CrosswordData[SelectedWordRow + i][SelectedWordColumn].Character == null
                                ? ' '
                                : CrosswordData[SelectedWordRow + i][SelectedWordColumn].Character;
                            i++;
                        }
                    }
                        break;
                    case WordDirection.Across:
                    {
                        while ((SelectedWordColumn + i < _crossword.Width) &&
                               (CrosswordData[SelectedWordRow][SelectedWordColumn + i].State !=
                                CellViewModel.CellState.Inactive))
                        {
                            word += CrosswordData[SelectedWordRow][SelectedWordColumn + i].Character == null
                                ? ' '
                                : CrosswordData[SelectedWordRow][SelectedWordColumn + i].Character;
                            i++;
                        }
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return word;
            }
        }

        public WordDirection SelectionDirection { get; set; }

        public SuggestionsViewModel SuggestionsDataModel { get; }

        public bool IsLayoutModeActive
        {
            get { return _isLayoutModeActive; }
            set
            {
                if (_isLayoutModeActive == value) return;
                _isLayoutModeActive = value;
                OnPropertyChanged(nameof(IsLayoutModeActive));
                OnPropertyChanged(nameof(IsSolvingModeActive));
            }
        }

        public bool IsSolvingModeActive => !IsLayoutModeActive;

        public int? Width { get; set; }

        public int? Height { get; set; }

        public ObservableCollection<MenuOptions> MenuItems { get; }

        public ObservableCollection<ObservableCollection<CellViewModel>> CrosswordData
        {
            get { return _crosswordData; }
            set
            {
                if (Equals(value, _crosswordData)) return;
                _crosswordData = value;
                OnPropertyChanged(nameof(CrosswordData));
            }
        }

        public ObservableCollection<ObservableCollection<SolidColorBrush>> GridColours { get; set; }

        public ICommand GoBackCommand => new DelegateCommand(obj => GoBack());
        public ICommand ShowLayoutGridCommand => new DelegateCommand(obj => ShowLayoutGrid());
        public ICommand ToNextModeCommand => new DelegateCommand(obj => ToNextMode());
        public ICommand KeyDownCommand => new DelegateCommand(obj => KeyDown(((string) obj)[0]));

        public bool IsMenuVisible
        {
            get { return _isMenuVisible; }
            private set
            {
                if (_isMenuVisible == value) return;
                _isMenuVisible = value;
                OnPropertyChanged(nameof(IsMenuVisible));
            }
        }

        public bool IsCrosswordVisible
        {
            get { return _isCrosswordVisible; }

            set
            {
                if (value == _isCrosswordVisible) return;
                _isCrosswordVisible = value;
                OnPropertyChanged(nameof(IsCrosswordVisible));
            }
        }

        public MenuOptions SelectedMenuItem
        {
            get { return MenuOptions.Nothing; }
            set
            {
                switch (value)
                {
                    case MenuOptions.NewCrossword:
                        PreviousState = new NavigationState(this);
                        IsDimensionsVisible = true;
                        IsMenuVisible = false;
                        break;
                    case MenuOptions.LoadCrossword:
                        PreviousState = new NavigationState(this);
                        IsMenuVisible = false;
                        var fileStream = FileSysDialog.LoadDialog();
                        if (fileStream == null)
                        {
                            GoBack();
                            return;
                        }
                        _crossword = Crossword.Load(fileStream);
                        CrosswordData = GetCrosswordData();
                        IsLayoutModeActive = false;
                        IsCrosswordVisible = true;
                        break;
                    case MenuOptions.Quit:
                        Application.Current.MainWindow.Hide();
                        Environment.Exit(0);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        public bool IsDimensionsVisible
        {
            get { return _isDimensionsVisible; }
            set
            {
                _isDimensionsVisible = value;
                OnPropertyChanged(nameof(IsDimensionsVisible));
            }
        }

        public bool IsBackVisible => PreviousState != null;

        private NavigationState PreviousState
        {
            get { return _previousState; }

            set
            {
                _previousState = value;
                OnPropertyChanged(nameof(IsBackVisible));
            }
        }

        public ICommand SaveCommand => new DelegateCommand(obj => Save());

        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateSuggestions() => SuggestionsDataModel.Suggest(SelectedWord);

        private void Save()
        {
            var filePath = FileSysDialog.SaveDialogue();
            if (filePath != null)
                _crossword.Save(filePath);
        }

        private void CrosswordCell_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(CellViewModel.Character)) return;
            char item;
            var cell = sender as CellViewModel;
            if (cell?.Character != null)
                item = (char) cell.Character;
            else
                item = ' ';
            if (cell?.State == CellViewModel.CellState.Inactive)
                item = '_';
            var found = false;
            int i, j = 0;
            for (i = 0; i < CrosswordData.Count; i++)
            {
                var row = CrosswordData[i];
                for (j = 0; j < row.Count; j++)
                {
                    var thing = row[j];
                    if (thing == sender)
                    {
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
            _crossword.Update(item, i, j);
        }

        private void KeyDown(char keyChar)
        {
            if (!IsSolvingModeActive || (SelectedRow == -1)) return;
            if ((keyChar >= 'A') && (keyChar <= 'Z'))
                CrosswordData[SelectedRow][SelectedColumn].Character = keyChar;
            else
                switch (keyChar)
                {
                    case ' ':
                        CrosswordData[SelectedRow][SelectedColumn].Character = null;
                        break;
                    case '\\':
                        CrosswordData[SelectedRow][SelectedColumn].Character = null;
                        switch (SelectionDirection)
                        {
                            case WordDirection.Down:
                                if ((SelectedRow - 1 >= 0) &&
                                    (CrosswordData[SelectedRow - 1][SelectedColumn].State !=
                                     CellViewModel.CellState.Inactive))
                                    CrosswordData[SelectedRow - 1][SelectedColumn].ButtonClickCommand.Execute(null);
                                break;
                            case WordDirection.Across:
                                if ((SelectedColumn - 1 >= 0) &&
                                    (CrosswordData[SelectedRow][SelectedColumn - 1].State !=
                                     CellViewModel.CellState.Inactive))
                                    CrosswordData[SelectedRow][SelectedColumn - 1].ButtonClickCommand.Execute(null);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        return;
                    case '0':
                        if ((SelectedRow - 1 >= 0) &&
                            (CrosswordData[SelectedRow - 1][SelectedColumn].State !=
                             CellViewModel.CellState.Inactive))
                            CrosswordData[SelectedRow - 1][SelectedColumn].ButtonClickCommand.Execute(null);
                        if (SelectionDirection == WordDirection.Across)
                            CrosswordData[SelectedRow][SelectedColumn].ButtonClickCommand.Execute(null);
                        return;
                    case '1':
                        if ((SelectedColumn + 1 < Width) &&
                            (CrosswordData[SelectedRow][SelectedColumn + 1].State !=
                             CellViewModel.CellState.Inactive))
                            CrosswordData[SelectedRow][SelectedColumn + 1].ButtonClickCommand.Execute(null);
                        if (SelectionDirection == WordDirection.Down)
                            CrosswordData[SelectedRow][SelectedColumn].ButtonClickCommand.Execute(null);
                        return;
                    case '2':
                        if ((SelectedRow + 1 < Height) &&
                            (CrosswordData[SelectedRow + 1][SelectedColumn].State !=
                             CellViewModel.CellState.Inactive))
                            CrosswordData[SelectedRow + 1][SelectedColumn].ButtonClickCommand.Execute(null);
                        if (SelectionDirection == WordDirection.Across)
                            CrosswordData[SelectedRow][SelectedColumn].ButtonClickCommand.Execute(null);
                        return;
                    case '3':
                        if ((SelectedColumn - 1 >= 0) &&
                            (CrosswordData[SelectedRow][SelectedColumn - 1].State !=
                             CellViewModel.CellState.Inactive))
                            CrosswordData[SelectedRow][SelectedColumn - 1].ButtonClickCommand.Execute(null);
                        if (SelectionDirection == WordDirection.Down)
                            CrosswordData[SelectedRow][SelectedColumn].ButtonClickCommand.Execute(null);
                        return;
                }

            switch (SelectionDirection)
            {
                case WordDirection.Down:
                    if ((SelectedRow + 1 < Width) &&
                        (CrosswordData[SelectedRow + 1][SelectedColumn].State != CellViewModel.CellState.Inactive))
                        CrosswordData[SelectedRow + 1][SelectedColumn].ButtonClickCommand.Execute(null);
                    break;
                case WordDirection.Across:
                    if ((SelectedColumn + 1 < Width) &&
                        (CrosswordData[SelectedRow][SelectedColumn + 1].State != CellViewModel.CellState.Inactive))
                        CrosswordData[SelectedRow][SelectedColumn + 1].ButtonClickCommand.Execute(null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ToNextMode()
        {
            if (!IsLayoutModeActive) return;
            PreviousState = new NavigationState(this);
            IsLayoutModeActive = false;
            //TODO: Generate Model
            _crossword = new Crossword(GetCrosswordLayoutData());
        }

        private char[,] GetCrosswordLayoutData()
        {
            var layoutData = new char[CrosswordData.Count, CrosswordData[0].Count];
            for (var i = 0; i < layoutData.GetLength(0); i++)
                for (var j = 0; j < layoutData.GetLength(1); j++)
                {
                    var cell = CrosswordData[i][j];
                    switch (cell.State)
                    {
                        case CellViewModel.CellState.Active:
                            layoutData[i, j] = ' ';
                            break;
                        case CellViewModel.CellState.Inactive:
                            layoutData[i, j] = '_';
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                $"Unhandled {nameof(CellViewModel.CellState)} case in switch statement.");
                    }
                }
            return layoutData;
        }

        private void ShowLayoutGrid()
        {
            if ((Height == null) || (Width == null)) return;

            PreviousState = new NavigationState(this);
            IsDimensionsVisible = false;

            CrosswordData = new ObservableCollection<ObservableCollection<CellViewModel>>();
            for (var i = 0; i < (int) Height; i++)
            {
                CrosswordData.Add(new ObservableCollection<CellViewModel>());
                for (var j = 0; j < (int) Width; j++)
                {
                    var cell = new CellViewModel(this);
                    cell.PropertyChanged += CrosswordCell_PropertyChanged;
                    CrosswordData[i].Add(cell);
                }
            }
            IsLayoutModeActive = true;
            IsCrosswordVisible = true;
        }

        private ObservableCollection<ObservableCollection<CellViewModel>> GetCrosswordData()
        {
            var collection = new ObservableCollection<ObservableCollection<CellViewModel>>();
            var crosswordData = _crossword.CrosswordData;

            Width = _crossword.Width;
            Height = _crossword.Height;

            for (var i = 0; i < _crossword.Height; i++)
            {
                var row = new ObservableCollection<CellViewModel>();
                for (var j = 0; j < _crossword.Width; j++)
                {
                    var cell = new CellViewModel(this)
                    {
                        Character = crosswordData[i, j],
                        State = CellViewModel.CellState.Active
                    };
                    if (cell.Character == '_')
                    {
                        cell.Character = null;
                        cell.State = CellViewModel.CellState.Inactive;
                    }
                    cell.PropertyChanged += CrosswordCell_PropertyChanged;
                    row.Add(cell);
                }
                collection.Add(row);
            }
            return collection;
        }

/*
                                                                                                                        private void Populate()
                                                                                                                        {
                                                                                                                            CrosswordData = new ObservableCollection<ObservableCollection<CellViewModel>>();
                                                                                                                            for (var i = 0; i < 12; i++)
                                                                                                                            {
                                                                                                                                var currentData = new ObservableCollection<CellViewModel>();
                                                                                                                                for (var j = 0; j < 12; j++)
                                                                                                                                {
                                                                                                                                    var cell = new CellViewModel(this) {Character = (char) ('a' + j)};
                                                                                                                                    cell.PropertyChanged += CrosswordCell_PropertyChanged;
                                                                                                                                    currentData.Add(cell);
                                                                                                                                }
                                                                                                                                CrosswordData.Add(currentData);
                                                                                                                            }
                                                                                                                        }
                                                                                                                */

        private void GoBack()
        {
            IsMenuVisible = PreviousState.IsMenuVisible;
            IsDimensionsVisible = PreviousState.IsDimensionsVisible;
            IsCrosswordVisible = PreviousState.IsCrosswordVisible;
            IsLayoutModeActive = PreviousState.IsLayoutModeActive;
            PreviousState = PreviousState.PreviousState;
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private class NavigationState
        {
            public readonly bool IsCrosswordVisible;
            public readonly bool IsDimensionsVisible;
            public readonly bool IsLayoutModeActive;
            public readonly bool IsMenuVisible;
            public readonly NavigationState PreviousState;

            public NavigationState(MainViewModel model)
            {
                IsMenuVisible = model._isMenuVisible;
                IsDimensionsVisible = model._isDimensionsVisible;
                IsCrosswordVisible = model.IsCrosswordVisible;
                IsLayoutModeActive = model.IsLayoutModeActive;
                PreviousState = model.PreviousState;
            }
        }
    }
}