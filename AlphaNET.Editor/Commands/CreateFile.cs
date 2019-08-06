using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Editor.Commands
{
    class CreateFile : Command
    {
        public CreateFile()
        {
            MenuText = "Create File";
            Shortcut = Application.Instance.CommonModifier | Keys.N;
        }
    }
}
