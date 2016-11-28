using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace SC.CrosswordSolver.UI.Views
{
    static class FileSysDialog
    {
        public static Stream LoadDialog()
        {
            var openDialog = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = ".crsword",
                Filter = "Crossword data files (.crsword)|*.crsword",
                Multiselect = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };
            openDialog.ShowDialog();
            return openDialog.OpenFile();
        }

        public static Stream SaveDialogue()
        {
            var saveDialog = new SaveFileDialog
            {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = ".crsword",
                Filter = "Crossword data files (.crsword)|*.crsword",
                OverwritePrompt = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = "crossword"
            };
            if (saveDialog.ShowDialog() ?? false)
                return saveDialog.OpenFile();
            else
            {
                return null;
            }
        }
    }
}