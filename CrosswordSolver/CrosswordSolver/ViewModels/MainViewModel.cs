using System;
using System.Collections.Generic;
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
        private bool _isBackVisible;

        public MainViewModel()
        {
            MenuItems = new ObservableCollection<MenuOptions>{MenuOptions.NewCrossword, MenuOptions.LoadCrossword, MenuOptions.Quit};
        }

        public int? Width { get; set; }
        public int? Height { get; set; }
        public ObservableCollection<MenuOptions> MenuItems { get; }

        public bool IsMenuVisible
        {
            get { return _isMenuVisible; }
            private set
            {
                _isMenuVisible = value;
                OnPropertyChanged(nameof(IsMenuVisible));
            }
        }

        public ICommand BackCommand { get; set; }

        public MenuOptions SelectedMenuItem
        {
            set
            {
                switch (value)
                {
                    case MenuOptions.NewCrossword:
                        IsDimensionsVisible = true;
                        IsBackVisible = true;
                        IsMenuVisible = false;
                        break;
                    case MenuOptions.LoadCrossword:
                        IsMenuVisible = false;
                        IsBackVisible = true;
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

        public bool IsBackVisible
        {
            get { return _isBackVisible; }
            set
            {
                _isBackVisible = value;
                OnPropertyChanged(nameof(IsBackVisible));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
