using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Editor.Commands
{
    class DeleteObject : Command
    {
        public DeleteObject()
        {
            MenuText = "Delete Filesystem Entry";
            Shortcut = Keys.Delete;
        }
    }
}
