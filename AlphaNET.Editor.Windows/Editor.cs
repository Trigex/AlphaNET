using AlphaNET.Editor.Forms;
using Eto.Forms;
using System;

namespace AlphaNET.Editor.Windows
{
    class Editor
    {
        [STAThread]
        static void Main(string[] args)
        {
            new Application(Eto.Platform.Detect).Run(new EditorForm());
        }
    }
}