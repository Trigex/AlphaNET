using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Editor.Commands
{
    class ImportAsset : Command
    {
        public ImportAsset()
        {
            MenuText = "Import Assets";
            Shortcut = Application.Instance.CommonModifier | Keys.I;
        }
    }
}
