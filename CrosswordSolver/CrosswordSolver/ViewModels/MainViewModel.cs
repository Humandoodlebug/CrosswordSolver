﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

            
        }

        public int? Width { get; set; }

        public int? Height { get; set; }

        private Crossword _crossword;
        private ObservableCollection<ObservableCollection<Cell>> _crosswordData;

        private void CrosswordDataMember_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var senderCollection = (ObservableCollection<Cell>)sender;

            if (e.NewItems.Count == 1)
            {
                char item;
                var cell = (Cell)e.NewItems[0];
                if (cell.Character == null)
                    item = ' ';
                else item = (char)cell.Character;

                _crossword.CrosswordData[CrosswordData.IndexOf(senderCollection), e.NewStartingIndex] = item;
            }
            else throw new ArgumentException("More than one change was made to the collection.");
        }

        public ObservableCollection<MenuOptions> MenuItems { get; }

        public ObservableCollection<ObservableCollection<Cell>> CrosswordData
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
        public ICommand GotoColourGrid => new DelegateCommand(obj => ShowColourGrid());

        public void ShowColourGrid()
        {
            
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

        private ObservableCollection<ObservableCollection<Cell>> GetCrosswordData()
        {
            var collection = new ObservableCollection<ObservableCollection<Cell>>();

            for (int i = 0; i < _crossword.Height; i++)
            {
                var row = new ObservableCollection<Cell>();
                for (int j = 0; j < _crossword.Width; j++)
                {
                    var cell = new Cell { Character = _crossword.CrosswordData[i, j], IsEnabled = true };
                    if (cell.Character == '-')
                    {
                        cell.Character = null;
                        cell.IsEnabled = false;
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

        //private void Populate()
        //{
        //    var random = new Random();
        //    for (var i = 0; i < 12; i++)
        //    {
        //        var currentData = new ObservableCollection<char?>();
        //        for (var j = 0; j < 12; j++)
        //            currentData.Add(random.Next(10).ToString()[0]);
        //        CrosswordData.Add(currentData);
        //    }
        //}

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

        public struct Cell : INotifyPropertyChanged
        {
            private bool _isEnabled;
            private char? _character;

            public bool IsEnabled
            {
                get { return _isEnabled; }

                set
                {
                    if (value == _isEnabled) return;
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }

            public char? Character
            {
                get { return _character; }

                set
                {
                    if (value == _character) return;
                    _character = value;
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
}