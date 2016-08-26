using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using SC.CrosswordSolver.UI.ViewModels;
using static SC.CrosswordSolver.UI.ViewModels.MainViewModel.Cell;

namespace SC.CrosswordSolver.UI.Converters
{
    public class CellStateVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cellState = (CellState) parameter;
            return cellState == CellState.Inactive ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
