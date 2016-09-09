using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SC.CrosswordSolver.Logic;
using SC.CrosswordSolver.UI.Annotations;
using SC.CrosswordSolver.UI.Model;
using SC.CrosswordSolver.UI.Views;

namespace SC.CrosswordSolver.UI.ViewModels
{

    public enum LayoutInteractionMode { InvertActive, InvertStarred }

    public enum WordDirection { Down, Across }

    public class MainViewModel : INotifyPropertyChanged
    {
        public WordDirection SelectionDirection { get; set; }
        public int SelectedWordRow = -1;
        public int SelectedWordColumn;


        private bool _isCrosswordVisible;
        private bool _isDimensionsVisible;
        private bool _isMenuVisible = true;
        private bool _isLayoutModeActive;

        private NavigationState _previousState;

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

        public MainViewModel()
        {
            MenuItems = new ObservableCollection<MenuOptions>
            {
                MenuOptions.NewCrossword,
                MenuOptions.LoadCrossword,
                MenuOptions.Quit
            };
            //Populate();
        }

        public int? Width { get; set; }

        public int? Height { get; set; }

        private Crossword _crossword;
        private ObservableCollection<ObservableCollection<CellViewModel>> _crosswordData;
        private LayoutInteractionMode _layoutGridMode;

        private void CrosswordDataMember_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var senderCollection = (ObservableCollection<CellViewModel>)sender;

            if (e.NewItems.Count == 1)
            {
                char item;
                var cell = (CellViewModel)e.NewItems[0];
                if (cell.Character == null)
                    item = ' ';
                else item = (char)cell.Character;
                _crossword.CrosswordData[CrosswordData.IndexOf(senderCollection), e.NewStartingIndex] = item;
            }
            else throw new ArgumentException("More than one change was made to the collection.");
        }

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

        public void ToNextMode()
        {
            if (!IsLayoutModeActive) return;
            PreviousState = new NavigationState(this);
            switch (LayoutGridMode)
            {
                case LayoutInteractionMode.InvertActive:
                    LayoutGridMode = LayoutInteractionMode.InvertStarred;
                    break;
                case LayoutInteractionMode.InvertStarred:
                    IsLayoutModeActive = false;
                    break;
            }
        }

        public void ShowLayoutGrid()
        {
            if (Height == null || Width == null) return;

            PreviousState = new NavigationState(this);
            IsDimensionsVisible = false;

            CrosswordData = new ObservableCollection<ObservableCollection<CellViewModel>>();
            for (int i = 0; i < (int) Height; i++)
            {
                CrosswordData.Add(new ObservableCollection<CellViewModel>());
                for (int j = 0; j < (int) Width; j++)
                {
                    CrosswordData[i].Add(new CellViewModel(this));
                }
            }
            IsLayoutModeActive = true;
            IsCrosswordVisible = true;
        }

        public LayoutInteractionMode LayoutGridMode
        {
            get { return _layoutGridMode; }
            set
            {
                if (value == _layoutGridMode) return;
                _layoutGridMode = value;
                OnPropertyChanged(nameof(LayoutGridMode));
            }
        }


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

        private ObservableCollection<ObservableCollection<CellViewModel>> GetCrosswordData()
        {
            var collection = new ObservableCollection<ObservableCollection<CellViewModel>>();

            for (int i = 0; i < _crossword.Height; i++)
            {
                var row = new ObservableCollection<CellViewModel>();
                for (int j = 0; j < _crossword.Width; j++)
                {
                    var cell = new CellViewModel(parentModel:this) { Character = _crossword.CrosswordData[i, j], IsEnabled = CellViewModel.CellState.Active };
                    if (cell.Character == '-')
                    {
                        cell.Character = null;
                        cell.IsEnabled = CellViewModel.CellState.Inactive;
                    }
                    row.Add(cell);
                }
                row.CollectionChanged += CrosswordDataMember_CollectionChanged;
                collection.Add(row);
            }
            return collection;
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
                        var filePath = FileSysDialog.LoadDialog();
                        if (!File.Exists(filePath))
                        {
                            GoBack();
                            return;
                        }
                        _crossword = Crossword.Load(filePath);
                        CrosswordData = GetCrosswordData();
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void Populate()
        {
            CrosswordData = new ObservableCollection<ObservableCollection<CellViewModel>>();
            for (var i = 0; i < 12; i++)
            {
                var currentData = new ObservableCollection<CellViewModel>();
                for (var j = 0; j < 12; j++)
                    currentData.Add(new CellViewModel(this) { Character = (char)('a' + j)});
                currentData.CollectionChanged += CrosswordDataMember_CollectionChanged;
                CrosswordData.Add(currentData);
            }
        }

        private void GoBack()
        {
            IsMenuVisible = PreviousState.IsMenuVisible;
            IsDimensionsVisible = PreviousState.IsDimensionsVisible;
            IsCrosswordVisible = PreviousState.IsCrosswordVisible;
            IsLayoutModeActive = PreviousState.IsLayoutModeActive;
            LayoutGridMode = PreviousState.LayoutGridMode;
            PreviousState = PreviousState.PreviousState;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private class NavigationState
        {
            public readonly bool IsCrosswordVisible;
            public readonly bool IsDimensionsVisible;
            public readonly bool IsMenuVisible;
            public readonly NavigationState PreviousState;
            public readonly bool IsLayoutModeActive;
            public LayoutInteractionMode LayoutGridMode;

            public NavigationState(MainViewModel model)
            {
                IsMenuVisible = model._isMenuVisible;
                IsDimensionsVisible = model._isDimensionsVisible;
                IsCrosswordVisible = model.IsCrosswordVisible;
                IsLayoutModeActive = model.IsLayoutModeActive;
                PreviousState = model.PreviousState;
                LayoutGridMode = model.LayoutGridMode;
            }
        }
    }
}