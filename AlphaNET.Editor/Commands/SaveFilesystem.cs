using Eto.Forms;

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
