using Eto.Forms;

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
