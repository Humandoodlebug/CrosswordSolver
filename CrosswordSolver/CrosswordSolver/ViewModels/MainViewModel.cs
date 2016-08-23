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
        private bool _isMenuVisible = true;
        private bool _isDimensionsVisible;

        public MainViewModel()
        {
            MenuItems = new ObservableCollection<MenuOptions>{MenuOptions.NewCrossword, MenuOptions.LoadCrossword, MenuOptions.Quit};
            GoBackCommand = new DelegateCommand(obj => GoBack());
        }

        public int? Width { get; set; }
        public int? Height { get; set; }
        public ObservableCollection<MenuOptions> MenuItems { get; }
        private MainViewModelState _previousState;
        public ICommand GoBackCommand { get; }

        public bool IsMenuVisible
        {
            get { return _isMenuVisible; }
            private set
            {
                _isMenuVisible = value;
                OnPropertyChanged(nameof(IsMenuVisible));
            }
        }

        private void GoBack()
        {
            IsMenuVisible = PreviousState.IsMenuVisible;
            IsDimensionsVisible = PreviousState.IsDimensionsVisible;
            PreviousState = PreviousState.PreviousState;
        }

        public MenuOptions SelectedMenuItem
        {
            get { return MenuOptions.Nothing; }
            set
            {
                switch (value)
                {
                    case MenuOptions.NewCrossword:
                        PreviousState = new MainViewModelState(this);
                        IsDimensionsVisible = true;
                        IsMenuVisible = false;
                        break;
                    case MenuOptions.LoadCrossword:
                        PreviousState = new MainViewModelState(this);
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

        private MainViewModelState PreviousState
        {
            get
            {
                return _previousState;
            }

            set
            {
                _previousState = value;
                OnPropertyChanged(nameof(IsBackVisible));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private class MainViewModelState
        {
            public readonly bool IsMenuVisible;
            public readonly bool IsDimensionsVisible;
            public readonly int? Width;
            public readonly int? Height;
            public readonly MainViewModelState PreviousState;

            public MainViewModelState(MainViewModel model)
            {
                IsMenuVisible = model._isMenuVisible;
                IsDimensionsVisible = model._isDimensionsVisible;
                Width = model.Width;
                Height = model.Height;
                PreviousState = model.PreviousState;
            }

        }
    }
}