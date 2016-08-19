using System.Collections.ObjectModel;

namespace SC.CrosswordSolver.UI.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            MenuItems = new ObservableCollection<string>{"New Crossword", "Load Crossword", "Quit"};
        }

        public int? Width { get; set; }
        public int? Height { get; set; }

        public ObservableCollection<string> MenuItems { get; }


    }
}
