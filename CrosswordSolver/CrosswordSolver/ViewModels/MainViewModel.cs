using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using SC.CrosswordSolver.UI.Annotations;
using SC.CrosswordSolver.UI.Model;

namespace SC.CrosswordSolver.UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _isMenuVisible;

        public MainViewModel()
        {
            MenuItems = new ObservableCollection<MenuOptions>{MenuOptions.NewCrossword, MenuOptions.LoadCrossword, MenuOptions.Quit};
            _isMenuVisible = true;
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

        public MenuOptions SelectedMenuItem
        {
            set
            {
                switch (value)
                {
                    case MenuOptions.NewCrossword:
                        IsMenuVisible = false;
                        break;
                    case MenuOptions.LoadCrossword:
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
