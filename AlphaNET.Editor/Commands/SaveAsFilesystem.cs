using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Editor.Commands
{
    class SaveAsFilesystem : Command
    {
        public SaveAsFilesystem()
        {
            MenuText = "Save As";
            Shortcut = Application.Instance.CommonModifier | Keys.Shift | Keys.S; // Ctrl + Shift + S
        }
    }
}
