﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using SC.CrosswordSolver.UI.Annotations;
using SC.CrosswordSolver.UI.Model;

namespace SC.CrosswordSolver.UI.ViewModels
{
    // TODO: Setup a selection state enum to keep track of whether the cell or a cell in its word is selected. Also implement this for visual selection states in the View.
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

        public CellSelectedState SelectionState
        {
            get { return _selectionState; }
            set
            {
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
                    GetWordStart(ref row, ref column, ParentModel.SelectionDirection);
                    value = CellSelectedState.WordSelected;
                    ParentModel.SelectedWordRow = row;
                    ParentModel.SelectedWordColumn = column;
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


                //if (value == CellSelectedState.WordSelected && _selectionState == CellSelectedState.Selected)
                //    return;
                //if (value == CellSelectedState.Selected)
                //{
                //    if ()
                //}
                //_selectionState = value;
                //OnPropertyChanged(nameof(SelectionState));
                //if (value == CellSelectedState.Selected)
                //    value = CellSelectedState.WordSelected;
                //int row, column;
                //GetPosition(out row, out column);
                //if (ParentModel.SelectionDirection == WordDirection.Down)
                //{
                //    if (row + 1 < ParentModel.CrosswordData.Count &&
                //        ParentModel.CrosswordData[row + 1][column].IsEnabled != CellState.Inactive)
                //        ParentModel.CrosswordData[row + 1][column].SelectionState = value;
                //}
                //else if (ParentModel.SelectionDirection == WordDirection.Across)
                //{
                //    if (column + 1 < ParentModel.CrosswordData[row].Count &&
                //        ParentModel.CrosswordData[row][column + 1].IsEnabled != CellState.Inactive)
                //        ParentModel.CrosswordData[row][column + 1].SelectionState = value;
                //}
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
                if (value.ToString().ToUpper()[0] == _character) return;
                _character = value.ToString().ToUpper()[0];
                OnPropertyChanged(nameof(Character));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void GetWordStart(ref int row, ref int column, WordDirection orientation)
        {
            if (orientation == WordDirection.Across)
                do
                {
                    column--;
                } while ((column > 0) && (ParentModel.CrosswordData[row][column - 1].IsEnabled != CellState.Inactive));

            else if (orientation == WordDirection.Down)
                do
                {
                    row--;
                } while ((row > 0) && (ParentModel.CrosswordData[row - 1][column].IsEnabled != CellState.Inactive));

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
                }

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