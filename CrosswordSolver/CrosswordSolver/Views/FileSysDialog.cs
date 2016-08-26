using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace SC.CrosswordSolver.UI.Views
{
    class FileSysDialog
    {
        public static string LoadDialog()
        {
            var openDialog = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = ".crsw",
                Filter = "Crossword data files (.crsw)|*.crsw",
            Multiselect = false
            };
            openDialog.ShowDialog();
            return openDialog.FileName;
        }

        public string SaveDialogue()
        {
            throw new NotImplementedException();
        }
    }
}
