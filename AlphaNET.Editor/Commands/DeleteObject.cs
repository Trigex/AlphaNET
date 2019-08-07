using Eto.Forms;

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
