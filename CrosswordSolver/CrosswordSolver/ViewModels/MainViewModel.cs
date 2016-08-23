using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using SC.CrosswordSolver.UI.Annotations;
using SC.CrosswordSolver.UI.Model;

namespace SC.CrosswordSolver.UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _isCrosswordVisible;
        private bool _isDimensionsVisible;
        private bool _isMenuVisible = true;

        private NavigationState _previousState;


        public MainViewModel()
        {
            MenuItems = new ObservableCollection<MenuOptions>
            {
                MenuOptions.NewCrossword,
                MenuOptions.LoadCrossword,
                MenuOptions.Quit
            };
            GoBackCommand = new DelegateCommand(obj => GoBack());
            Populate();
        }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public ObservableCollection<MenuOptions> MenuItems { get; }

        public ObservableCollection<ObservableCollection<char>> CrosswordData { get; set; } =
            new ObservableCollection<ObservableCollection<char>>();

        public ICommand GoBackCommand { get; }

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
                OnPropertyChanged();
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
            var random = new Random();
            for (var i = 0; i < 12; i++)
            {
                var currentData = new ObservableCollection<char>();
                for (var j = 0; j < 12; j++)
                    currentData.Add(random.Next(10).ToString()[0]);
                CrosswordData.Add(currentData);
            }
        }

        private void GoBack()
        {
            IsMenuVisible = PreviousState.IsMenuVisible;
            IsDimensionsVisible = PreviousState.IsDimensionsVisible;
            IsCrosswordVisible = PreviousState.IsCrosswordVisible;
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

            public NavigationState(MainViewModel model)
            {
                IsMenuVisible = model._isMenuVisible;
                IsDimensionsVisible = model._isDimensionsVisible;
                PreviousState = model.PreviousState;
                IsCrosswordVisible = model.IsCrosswordVisible;
            }
        }
    }
}