using AlphaNET.Editor.GridItems;
using AlphaNET.Framework.IO;
using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
                        var item = r.Item as FilesystemObjectGridItem;
                        var label = new Label() { Text = item.Title };
                        return label;
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
                        Label label = new Label();

                        if (item.GetType() == typeof(DirectoryGridItem))
                        {
                            label.Text = "Directory";
                        }
                        else if (item.GetType() == typeof(FileGridItem))
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
                        Label label = new Label();
                        // only apply to file items
                        if (item.GetType() == typeof(FileGridItem))
                        {
                            var fgi = (FileGridItem)item;
                            if (fgi.IsPlaintext)
                            {
                                label.Text = "Text";
                            }
                            else
                            {
                                label.Text = "Binary";
                            }
                        }

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
                        var item = r.Item as FilesystemObjectGridItem;
                        Label label = new Label { Text = item.Size.ToString() };

                        return label;
                    }
                }
            });
        }

        public void LoadFilesystem(Filesystem fs)
        {
            // if attempting to load from an already active session
            if (DataStore != null && treeViewItems != null)
            {
                treeViewItems.Clear();
            }

            var root = (Directory)fs.GetFilesystemObjectsByTitle("root")[0];
            var rootItem = new DirectoryGridItem(root.ID, root.Title, root);
            FilesystemTraverse(root, rootItem);
            rootItem.Size = rootItem.GetDirectorySize((Directory)fs.GetFilesystemObjectByID(root.ID), 0);
            treeViewItems.Add(rootItem);
            DataStore = treeViewItems;
        }

        private void FilesystemTraverse(Directory dir, FilesystemObjectGridItem lastItem)
        {
            foreach (FilesystemObject child in dir.Children)
            {
                if (child.GetType() == typeof(File))
                {
                    File childFile = (File)child;
                    FileGridItem fileItem = new FileGridItem(childFile.ID, childFile.Title, childFile.IsPlaintext, childFile);
                    lastItem.Children.Add(fileItem);
                }

                if (child.GetType() == typeof(Directory))
                {
                    Directory childDir = (Directory)child;

                    DirectoryGridItem dirItem = new DirectoryGridItem(childDir.ID, childDir.Title, childDir);
                    lastItem.Children.Add(dirItem);

                    FilesystemTraverse((Directory)child, dirItem);
                }
            }
        }
    }
}
