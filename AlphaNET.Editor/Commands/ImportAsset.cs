using Eto.Forms;

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
