using System;
using System.Collections.Generic;
using System.Text;
using AlphaNET.Framework.Standard.IO;
using AlphaNET.Editor.Commands;
using Eto.Forms;
using Eto.Drawing;
using System.Linq;
using System.Diagnostics;

namespace AlphaNET.Editor.Forms
{
    public class EditorForm : Form
    {
        private Filesystem fs;
        private FileFilter _fsFilter;
        private Uri initalDirectory;
        private const string baseTitle = "AlphaNET Editor";

        private TreeGridView treeView;
        private TreeGridItemCollection treeViewItems;
        private TextArea textArea;
        private string currentlyEditedPath;
        private File currentlyEditedFile;
        public Command openFs, saveFs, saveAs, createFile, importAst, deleteObj;

        public EditorForm()
        {
            initalDirectory = new Uri(System.IO.Directory.GetCurrentDirectory());

            _fsFilter = new FileFilter("Filesystem (*.fs)", new[] { ".fs" });
            InitCommands();
            InitControls();
            InitInterface();
        }

        private void InitCommands()
        {
            openFs = new OpenFilesystem();
            saveFs = new SaveFilesystem();
            importAst = new ImportAsset();
            deleteObj = new DeleteObject();
            saveAs = new SaveAsFilesystem();
            createFile = new CreateFile();

            openFs.Executed += OpenFilesystem;
            saveFs.Executed += SaveFilesystem;
            importAst.Executed += ImportAsset;
            deleteObj.Executed += DeleteObject;
            saveAs.Executed += SaveAs;
            createFile.Executed += CreateFile;
        }

        private void InitControls()
        {
            treeViewItems = new TreeGridItemCollection();

            treeView = new TreeGridView()
            {
                BackgroundColor = Colors.White
            };
            // FILESYSTEM OBJECT TITLE TREE
            treeView.Columns.Add(new GridColumn
            {
                HeaderText = "Filesystem",
                Width = 250,
                DataCell = new CustomCell
                {
                    CreateCell = r =>
                    {
                        var item = r.Item as TreeGridItem;
                        var label = new Label() { Text = item.Values[0] as string,  };
                        return label;
                    }
                }
            });
            // FILESYSTEM OBJECT TYPE
            treeView.Columns.Add(new GridColumn
            {
                HeaderText = "Type",
                Width = 95,
                DataCell = new CustomCell
                {
                    CreateCell = r =>
                    {
                        var item = r.Item as TreeGridItem;
                        Label label = new Label();

                        if(fs.GetFilesystemObjectByID((uint)item.Tag).GetType() == typeof(Directory))
                        {
                            label.Text = "Directory";
                        }
                        if (fs.GetFilesystemObjectByID((uint)item.Tag).GetType() == typeof(File))
                        {
                            label.Text = "File";
                        }

                        return label;
                    }
                }
            });
            // FILE CONTENTS TYPE
            treeView.Columns.Add(new GridColumn
            {
                HeaderText = "Contents Type",
                DataCell = new CustomCell
                {
                    CreateCell = r =>
                    {
                        var item = r.Item as TreeGridItem;
                        Label label = new Label();
                        // only apply to file items
                        if(fs.GetFilesystemObjectByID((uint)item.Tag).GetType() == typeof(File))
                        {
                            if((string)item.Values[1] == true.ToString())
                            {
                                label.Text = "Text";
                            } else
                            {
                                label.Text = "Binary";
                            }
                        }

                        return label;
                    }
                }
            });
            treeView.Activated += TreeItemActivated;

            textArea = new TextArea();
            textArea.TextChanged += TextChanged;
        }

        private void InitInterface()
        {
            Title = baseTitle;
            ClientSize = new Eto.Drawing.Size(800, 600);
            // FORM CONTENTS
            Content = new TableLayout
            {
                Spacing = new Eto.Drawing.Size(5, 5),
                Padding = new Eto.Drawing.Padding(10, 10, 10, 10),
                Rows =
                {
                     new TableRow() { ScaleHeight = true, Cells = { treeView } },
                     new TableRow() { ScaleHeight = true, Cells = { textArea } },
                }
            };

            // MENUBAR
            Menu = new MenuBar
            {
                Items =
                {
                    new ButtonMenuItem // FILE MENU
                    {
                        Text = "File",
                        Items =
                        {
                            openFs,
                            saveFs,
                            saveAs
                        }
                    },

                    new ButtonMenuItem // EDIT MENU
                    {
                        Text = "Edit",
                        Items =
                        {
                            importAst,
                            deleteObj,
                            createFile
                        }
                    }
                }
            };
        }

        // COMMAND HANDLERS

        private void OpenFilesystem(object sender, EventArgs e)
        {
            var openFile = new OpenFileDialog();
            openFile.Filters.Add(_fsFilter);
            openFile.CurrentFilter = _fsFilter;
            openFile.MultiSelect = false;
            openFile.Directory = initalDirectory;

            if ( !(openFile.ShowDialog(this) == DialogResult.Ok && openFile.FileName.Contains(".fs")) )
            {
                MessageBox.Show("Please select a valid filesystem.", MessageBoxType.Error);
                return;
            }

            // Load fs
            fs = BinaryManager.CreateFilesystemFromBinary(BinaryManager.ReadBinaryFromFile(openFile.FileName));
            currentlyEditedPath = openFile.FileName;
            LoadFilesystemIntoTreeView();
            Title = baseTitle + " - " + System.IO.Path.GetFileName(currentlyEditedPath);
            textArea.Text = "";
        }

        private void SaveFilesystem(object sender, EventArgs e)
        {
            if(currentlyEditedPath != null)
                BinaryManager.WriteBinaryToFile(currentlyEditedPath, BinaryManager.CreateBinaryFromFilesystem(fs));
        }

        private void SaveAs(object sender, EventArgs e)
        {
            if(fs != null)
            {
                var saveFile = new SaveFileDialog();
                saveFile.Filters.Add(_fsFilter);
                saveFile.CurrentFilter = _fsFilter;
                saveFile.Directory = initalDirectory;
                var result = saveFile.ShowDialog(this);

                if (result == DialogResult.Ok)
                {
                    BinaryManager.WriteBinaryToFile(saveFile.FileName, BinaryManager.CreateBinaryFromFilesystem(fs));
                } else if(result != DialogResult.Cancel)
                {
                    MessageBox.Show("Unable to save.", MessageBoxType.Error);
                    return;
                }     
            }
        }

        private void ImportAsset(object sender, EventArgs e)
        {
            if(fs != null)
            {
                TreeGridItem selectedItem = (TreeGridItem)treeView.SelectedItem;
                TreeGridItem selectedDir;
                Directory importDirectory;
                // if the selected item isn't a directory, set selected to the item's parent (the directory whom holds it)
                if (!IsTreeGridItemDirectory(selectedItem))
                {
                    selectedDir = (TreeGridItem)selectedItem.Parent;
                }
                else
                {
                    selectedDir = selectedItem;
                }
                importDirectory = (Directory)fs.GetFilesystemObjectByID((uint)selectedDir.Tag);

                var openFile = new OpenFileDialog();
                openFile.MultiSelect = true;
                openFile.Directory = initalDirectory;

                if (!(openFile.ShowDialog(this) == DialogResult.Ok))
                {
                    MessageBox.Show("Unable to import asset.", MessageBoxType.Error);
                    return;
                }

                Dictionary<string, byte[]> bins = new Dictionary<string, byte[]>();

                // Load selected files
                foreach (var path in openFile.Filenames)
                {
                    bins.Add(System.IO.Path.GetFileName(path), BinaryManager.ReadBinaryFromFile(path));
                }

                // Create fs files
                foreach (var bin in bins)
                {
                    bool plaintext;
                    // not the greatest check, but it'll work for now. determine if it's text or a binary
                    if (bin.Key.Contains(".txt") || bin.Key.Contains(".ts") || bin.Key.Contains(".js"))
                        plaintext = true;
                    else
                        plaintext = false;


                    var file = new File(bin.Key, importDirectory, IOUtils.GenerateID(), plaintext, bin.Value);
                    fs.AddFilesystemObject(file, importDirectory);
                    selectedDir.Children.Add(new TreeGridItem { Tag = file.ID, Values = new string[] { file.Title, file.IsPlaintext.ToString() } });
                }

                selectedDir.Expanded = true;
                // refresh view
                treeView.ReloadData();
            }
        }

        private void CreateFile(object sender, EventArgs e)
        {
            if (fs != null)
            {
            }
        }

        private void DeleteObject(object sender, EventArgs e)
        {
            if(fs != null)
            {
                var selectedItem = (TreeGridItem)treeView.SelectedItem;
                var parent = (TreeGridItem)selectedItem.Parent;
                var fsObj = fs.GetFilesystemObjectByID((uint)selectedItem.Tag);
                // remove from tree
                if (selectedItem.Parent == null) // probably root
                    MessageBox.Show("Filesystems are required to have a root directory", MessageBoxType.Error);
                else
                {
                    parent.Children.Remove(selectedItem);

                    // remove from filesystem
                    fs.DeleteFilesystemObject(fsObj);

                    // reload
                    treeView.ReloadData();
                }
            }
        }

        // CONTROL HANDLERS
        private void TreeItemActivated(object sender, EventArgs e)
        {
            // reset text area
            currentlyEditedFile = null;
            textArea.Text = "";
            var selectedItem = (TreeGridItem)treeView.SelectedItem;
            if(IsTreeGridItemFile(selectedItem))
            {
                File file = (File)fs.GetFilesystemObjectByID((uint)selectedItem.Tag);
                this.currentlyEditedFile = file;
                textArea.Text = Encoding.UTF8.GetString(file.Contents);
            }
        }

        private void TreeCell(object sender, EventArgs e)
        {

        }

        private void TextChanged(object sender, EventArgs e)
        {
            // modify file contents
            if(currentlyEditedFile != null)
                currentlyEditedFile.ModifyContents(Encoding.UTF8.GetBytes(textArea.Text), true);
        }

        // VARIOUS METHODS
        private void LoadFilesystemIntoTreeView()
        {
            // if attempting to load from an already active session
            if(treeView.DataStore != null && treeViewItems != null)
            {
                treeViewItems.Clear();
            }

            var root = (Directory)fs.GetFilesystemObjectsByTitle("root")[0];
            var rootItem = new TreeGridItem { Tag = root.ID, Values = new string[] { root.Title } };
            FilesystemTraverse(root, rootItem);
            treeViewItems.Add(rootItem);

            treeView.DataStore = treeViewItems;
        }

        private void FilesystemTraverse(Directory dir, TreeGridItem lastItem)
        {
            foreach (FilesystemObject child in dir.Children)
            {
                if(child.GetType()==typeof(File))
                {
                    File childFile = (File)child;
                    TreeGridItem fileItem = new TreeGridItem();
                    fileItem.Tag = childFile.ID;
                    fileItem.Values = new string[] { childFile.Title, childFile.IsPlaintext.ToString() };
                    lastItem.Children.Add(fileItem);
                }

                if(child.GetType()==typeof(Directory))
                {
                    Directory childDir = (Directory)child;

                    TreeGridItem dirItem = new TreeGridItem();
                    dirItem.Tag = childDir.ID;
                    dirItem.Values = new string[] { childDir.Title };
                    lastItem.Children.Add(dirItem);

                    FilesystemTraverse((Directory)child, dirItem);
                }
            }
        }

        private bool IsTreeGridItemFile(TreeGridItem item)
        {
            if(fs.GetFilesystemObjectByID((uint)item.Tag).GetType() == typeof(File))
            {
                return true;
            } else
            {
                return false;
            }
        }

        private bool IsTreeGridItemDirectory(TreeGridItem item)
        {
            if (fs.GetFilesystemObjectByID((uint)item.Tag).GetType() == typeof(Directory))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
