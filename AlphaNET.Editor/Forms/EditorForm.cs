using System;
using System.Collections.Generic;
using System.Text;
using AlphaNET.Framework.IO;
using AlphaNET.Editor.Commands;
using Eto.Forms;
using AlphaNET.Editor.GridItems;
using AlphaNET.Editor.Controls;
using AlphaNET.Editor.Layouts;

namespace AlphaNET.Editor.Forms
{
    public class EditorForm : Form
    {
        private Filesystem fs;
        private FileFilter _fsFilter;
        private Uri initalDirectory;
        private const string baseTitle = "AlphaNET Editor";

        private FilesystemView fsView;
        private TextArea textArea;
        private string currentlyEditedPath;
        private File currentlyEditedFile;
        private Command openFs, saveFs, saveAs, createFile, importAst, deleteObj;

        public EditorForm()
        {
            initalDirectory = new Uri(System.IO.Directory.GetCurrentDirectory());

            _fsFilter = new FileFilter("Filesystem (*.fs)", new[] { ".fs" });
            InitCommands();
            InitControls();
            InitInterface();
        }

        // INIT METHODS

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
            fsView = new FilesystemView();
            fsView.Activated += FsViewItemActivated;

            textArea = new TextArea();
            textArea.TextChanged += TextChanged;
        }

        private void InitInterface()
        {
            Title = baseTitle;
            ClientSize = new Eto.Drawing.Size(800, 600);
            // FORM CONTENTS
            Content = EditorLayout.CreateInstance(fsView, textArea);

            // MENUBAR
            Dictionary<string, Command[]> items = new Dictionary<string, Command[]>();
            items.Add("File", new Command[] { openFs, saveFs, saveAs });
            items.Add("Edit", new Command[] { importAst, createFile, deleteObj });

            Menu = MenuLayout.CreateInstance(items);
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
            fsView.LoadFilesystem(fs);
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
                saveFile.Directory = new Uri(currentlyEditedPath);
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
                FilesystemObjectGridItem selectedItem = (FilesystemObjectGridItem)fsView.SelectedItem;
                Directory importDirectory;
                // if the selected item isn't a directory, set selected to the item's parent (the directory whom holds it)
                if (selectedItem.GetType() != typeof(DirectoryGridItem))
                {
                    var parentDir = (DirectoryGridItem)selectedItem.Parent;
                    importDirectory = (Directory)parentDir.FilesystemObject;
                    selectedItem = (FilesystemObjectGridItem)selectedItem.Parent;
                }
                else // the selected item is a directory
                {
                    importDirectory = (Directory)selectedItem.FilesystemObject;
                }

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
                    var fileItem = new FileGridItem(file.ID, file.Title, file.IsPlaintext, file);
                    fs.AddFilesystemObject(file, importDirectory);

                    selectedItem.Children.Add(fileItem);
                }

                selectedItem.Expanded = true;
                // refresh view
                fsView.ReloadData();
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
                var selectedItem = (FilesystemObjectGridItem)fsView.SelectedItem;
                var parent = (FilesystemObjectGridItem)selectedItem.Parent;
                var fsObj = selectedItem.FilesystemObject;

                // remove from tree
                if (selectedItem.Parent == null) // probably root
                    MessageBox.Show("Filesystems are required to have a root directory", MessageBoxType.Error);
                else
                {
                    parent.Children.Remove(selectedItem);
                    // remove from filesystem
                    fs.DeleteFilesystemObject(fsObj);
                    // reload
                    fsView.ReloadData();
                }
            }
        }

        // CONTROL HANDLERS
        private void FsViewItemActivated(object sender, EventArgs e)
        {
            // reset text area
            currentlyEditedFile = null;
            textArea.Text = "";
            var selectedItem = (FilesystemObjectGridItem)fsView.SelectedItem;
            if(selectedItem.GetType() == typeof(FileGridItem))
            {
                File file = (File)selectedItem.FilesystemObject;
                currentlyEditedFile = file;
                textArea.Text = Encoding.UTF8.GetString(file.Contents);
            }
        }

        private void TextChanged(object sender, EventArgs e)
        {
            // modify file contents
            if(currentlyEditedFile != null)
                currentlyEditedFile.ModifyContents(Encoding.UTF8.GetBytes(textArea.Text), true);
        }
    }
}
