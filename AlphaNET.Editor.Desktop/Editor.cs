using System;
using Eto.Forms;
using AlphaNET.Editor.Forms;
using Eto.Drawing;

namespace AlphaNET.Editor.Desktop
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