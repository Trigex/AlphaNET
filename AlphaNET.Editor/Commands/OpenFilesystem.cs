using Eto.Forms;
using System;
using AlphaNET.Framework.IO;
using System.Collections.Generic;
using System.Text;
using AlphaNET.Editor.Forms;

namespace AlphaNET.Editor.Commands
{
    class OpenFilesystem : Command
    {
        public OpenFilesystem()
        {
            MenuText = "Open Filesystem";
            Shortcut = Application.Instance.CommonModifier | Keys.O;
        }
    }
}
