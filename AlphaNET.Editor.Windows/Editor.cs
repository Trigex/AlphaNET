using AlphaNET.Editor.Forms;
using Eto.Forms;
using System;

namespace AlphaNET.Editor.Windows
{
    internal class Editor
    {
        [STAThread]
        private static void Main()
        {
            using (var app = new Application(Eto.Platform.Detect))
            {
                app.Run(new EditorForm());
            }
        }
    }
}