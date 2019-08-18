using System;
using AlphaNET.Editor.GridItems;
using AlphaNET.Framework.IO;
using Eto.Drawing;
using Eto.Forms;

namespace AlphaNET.Editor.Controls
{
    class FilesystemView : TreeGridView
    {
        public TreeGridItemCollection treeViewItems { get; set; }

        public FilesystemView() : base()
        {
            treeViewItems = new TreeGridItemCollection();
            BackgroundColor = Colors.White;
            // FILESYSTEM OBJECT TITLE TREE
            Columns.Add(new GridColumn
            {
                HeaderText = "Filesystem",
                Width = 250,
                DataCell = new CustomCell
                {
                    CreateCell = r =>
                    {
                        if (r.Item is FilesystemObjectGridItem item)
                        {
                            var label = new Label() { Text = item.Title };
                            return label;
                        }

                        return new Label();
                    }
                }
            });
            // FILESYSTEM OBJECT TYPE
            Columns.Add(new GridColumn
            {
                HeaderText = "Type",
                Width = 95,
                DataCell = new CustomCell
                {
                    CreateCell = r =>
                    {
                        var item = r.Item as FilesystemObjectGridItem;
                        var label = new Label();

                        if (item != null && item.GetType() == typeof(DirectoryGridItem))
                        {
                            label.Text = "Directory";
                        }
                        else if (item != null && item.GetType() == typeof(FileGridItem))
                        {
                            label.Text = "File";
                        }

                        return label;
                    }
                }
            });
            // FILE CONTENTS TYPE
            Columns.Add(new GridColumn
            {
                HeaderText = "Contents Type",
                DataCell = new CustomCell
                {
                    CreateCell = r =>
                    {
                        var item = r.Item as FilesystemObjectGridItem;
                        var label = new Label();
                        // only apply to file items
                        if (item != null && item.GetType() != typeof(FileGridItem)) return label;
                        var fgi = (FileGridItem)item;
                        label.Text = fgi != null && fgi.IsPlaintext ? "Text" : "Binary";

                        return label;
                    }
                }
            });
            // SIZE
            Columns.Add(new GridColumn
            {
                HeaderText = "Size (Bytes)",
                DataCell = new CustomCell
                {
                    CreateCell = r =>
                    {
                        if (!(r.Item is FilesystemObjectGridItem item)) return new Label();
                        return new Label { Text = item.Size.ToString() };
                    }
                }
            });
        }

        public void LoadFilesystem(Filesystem fs)
        {
            // if attempting to load from an already active session
            if (DataStore != null)
            {
                treeViewItems?.Clear();
            }

            var root = (Directory)fs.GetObjectsByTitle("root")[0];
            var rootItem = new DirectoryGridItem(root.Id, root.Title, root);
            FilesystemTraverse(root, rootItem);
            rootItem.Size = rootItem.GetDirectorySize((Directory)fs.GetObjectById(root.Id), 0);
            treeViewItems.Add(rootItem);
            DataStore = treeViewItems;
        }

        private void FilesystemTraverse(Directory dir, FilesystemObjectGridItem lastItem)
        {
            if (lastItem == null) throw new ArgumentNullException(nameof(lastItem));
            foreach (var child in dir.Children)
            {
                if (child.GetType() == typeof(File))
                {
                    var childFile = (File)child;
                    var fileItem = new FileGridItem(childFile.Id, childFile.Title, childFile.IsPlaintext, childFile);
                    lastItem.Children.Add(fileItem);
                }

                if (child.GetType() != typeof(Directory)) continue;
                var childDir = (Directory)child;

                var dirItem = new DirectoryGridItem(childDir.Id, childDir.Title, childDir);
                lastItem.Children.Add(dirItem);

                FilesystemTraverse((Directory)child, dirItem);
            }
        }
    }
}
