using System.ComponentModel;

namespace SC.CrosswordSolver.UI.Model
{
    public enum MenuOptions
    {
        Nothing,

        [Description("New Crossword")]
        NewCrossword,

        [Description("Load Crossword")]
        LoadCrossword,

        [Description("Quit")]
        Quit
    }
}
