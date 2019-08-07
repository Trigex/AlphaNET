using Eto.Forms;

namespace AlphaNET.Editor.Commands
{
    class CreateFile : Command
    {
        public CreateFile()
        {
            MenuText = "Create File";
            Shortcut = Application.Instance.CommonModifier | Keys.N;
        }
    }
}
