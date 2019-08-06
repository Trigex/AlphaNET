using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Editor.Commands
{
    class SaveFilesystem : Command
    {
        public SaveFilesystem()
        {
            MenuText = "Save Filesystem";
            Shortcut = Application.Instance.CommonModifier | Keys.S;
        }
    }
}
