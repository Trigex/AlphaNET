using AlphaNET.Editor.Controls;
using Eto.Forms;
using System.Collections.ObjectModel;

namespace AlphaNET.Editor.Layouts
{
    class EditorLayout : TableLayout
    {
        public static EditorLayout CreateInstance(FilesystemView fsView, TextArea textArea)
        {
            Collection<TableRow> rowLayout = new Collection<TableRow>();
            rowLayout.Add(new TableRow { ScaleHeight = true, Cells = { fsView } });
            rowLayout.Add(new TableRow { ScaleHeight = true, Cells = { textArea } });
            return new EditorLayout(rowLayout);
        }

        public EditorLayout(Collection<TableRow> rows) : base(rows)
        {
            Spacing = new Eto.Drawing.Size(5, 5);
            Padding = new Eto.Drawing.Padding(10, 10, 10, 10);
        }
    }
}
