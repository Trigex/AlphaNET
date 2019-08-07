using Eto.Forms;

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
