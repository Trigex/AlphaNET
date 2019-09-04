using AlphaNET.Editor.Commands;
using AlphaNET.Editor.Controls;
using AlphaNET.Editor.GridItems;
using AlphaNET.Editor.Layouts;
using AlphaNET.Framework.IO;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphaNET.Editor.Forms
{
    public class EditorForm : Form
    {
        private Filesystem _fs;
        private readonly FileFilter _fsFilter;
        private readonly Uri _initalDirectory;
        private const string BaseTitle = "AlphaNET Editor";

        private ContextMenu _ctxMenu;
        private FilesystemView _fsView;
        private TextArea _textArea;
        private string _currentlyEditedPath;
        private File _currentlyEditedFile;
        private Command _openFs, _saveFs, _saveAs, _createFile, _importAst, _deleteObj;

        public EditorForm()
        {
            _initalDirectory = new Uri(System.IO.Directory.GetCurrentDirectory());

            _fsFilter = new FileFilter("Filesystem (*.fs)", new[] { ".fs" });
            InitCommands();
            InitControls();
            InitInterface();
        }

        // INIT METHODS

        private void InitCommands()
        {
            _openFs = new OpenFilesystem();
            _saveFs = new SaveFilesystem();
            _importAst = new ImportAsset();
            _deleteObj = new DeleteObject();
            _saveAs = new SaveAsFilesystem();
            _createFile = new CreateFile();

            _openFs.Executed += OpenFilesystem;
            _saveFs.Executed += SaveFilesystem;
            _importAst.Executed += ImportAsset;
            _deleteObj.Executed += DeleteObject;
            _saveAs.Executed += SaveAs;
            _createFile.Executed += CreateFile;

            _ctxMenu = new ContextMenu { Items = { _deleteObj, _createFile } };
        }

        private void InitControls()
        {
            _fsView = new FilesystemView();
            _fsView.Activated += FsViewItemActivated;
            _fsView.MouseDown += FsViewMouseDown;

            _textArea = new TextArea();
            _textArea.TextChanged += TextChanged;
        }

        private void InitInterface()
        {
            Title = BaseTitle;
            ClientSize = new Eto.Drawing.Size(800, 600);
            // FORM CONTENTS
            Content = EditorLayout.CreateInstance(_fsView, _textArea);

            // MENUBAR
            var items = new Dictionary<string, Command[]>
            {
                {"File", new[] {_openFs, _saveFs, _saveAs}},
                {"Edit", new[] {_importAst, _createFile, _deleteObj}}
            };

            Menu = MenuLayout.CreateInstance(items);
        }

        // COMMAND HANDLERS

        private void OpenFilesystem(object sender, EventArgs e)
        {
            var openFile = new OpenFileDialog();
            openFile.Filters.Add(_fsFilter);
            openFile.CurrentFilter = _fsFilter;
            openFile.MultiSelect = false;
            openFile.Directory = _initalDirectory;

            if (!(openFile.ShowDialog(this) == DialogResult.Ok && openFile.FileName.Contains(".fs")))
            {
                MessageBox.Show("Please select a valid filesystem.", MessageBoxType.Error);
                return;
            }

            // Load fs
            _fs = BinaryManager.CreateFilesystemFromBinary(BinaryManager.ReadBinaryFromFile(openFile.FileName), openFile.FileName);
            _currentlyEditedPath = openFile.FileName;
            _fsView.LoadFilesystem(_fs);
            Title = BaseTitle + " - " + System.IO.Path.GetFileName(_currentlyEditedPath);
            _textArea.Text = "";
        }

        private void SaveFilesystem(object sender, EventArgs e)
        {
            if (_currentlyEditedPath != null)
            {
                BinaryManager.WriteBinaryToFile(_currentlyEditedPath, BinaryManager.CreateBinaryFromFilesystem(_fs));
            }
        }

        private void SaveAs(object sender, EventArgs e)
        {
            if (_fs == null) return;
            var saveFile = new SaveFileDialog();
            saveFile.Filters.Add(_fsFilter);
            saveFile.CurrentFilter = _fsFilter;
            saveFile.Directory = new Uri(_currentlyEditedPath);
            var result = saveFile.ShowDialog(this);

            if (result == DialogResult.Ok)
            {
                BinaryManager.WriteBinaryToFile(saveFile.FileName, BinaryManager.CreateBinaryFromFilesystem(_fs));
            }
            else if (result != DialogResult.Cancel)
            {
                MessageBox.Show("Unable to save.", MessageBoxType.Error);
                return;
            }
        }

        private void ImportAsset(object sender, EventArgs e)
        {
            if (_fs == null) return;
            FilesystemObjectGridItem selectedItem = (FilesystemObjectGridItem)_fsView.SelectedItem;
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

            var openFile = new OpenFileDialog {MultiSelect = true, Directory = _initalDirectory};

            if (openFile.ShowDialog(this) != DialogResult.Ok)
            {
                MessageBox.Show("Unable to import asset.", MessageBoxType.Error);
                return;
            }

            var bins = openFile.Filenames.ToDictionary(path => System.IO.Path.GetFileName(path) ?? throw new InvalidOperationException(), BinaryManager.ReadBinaryFromFile);

            // Create fs files
            foreach (var bin in bins)
            {
                bool plaintext;
                // not the greatest check, but it'll work for now. determine if it's text or a binary
                if (bin.Key.Contains(".txt") || bin.Key.Contains(".ts") || bin.Key.Contains(".js"))
                {
                    plaintext = true;
                }
                else
                {
                    plaintext = false;
                }

                var file = new File(bin.Key, IoUtils.GenerateId(), plaintext, bin.Value);
                var fileItem = new FileGridItem(file.Id, file.Title, file.IsPlaintext, file);
                _fs.AddObject(file, importDirectory);

                selectedItem.Children.Add(fileItem);
            }

            selectedItem.Expanded = true;
            // refresh view
            _fsView.ReloadData();
        }

        private void CreateFile(object sender, EventArgs e)
        {
            if (_fs == null) return;
            FilesystemObjectGridItem selectedItem = (FilesystemObjectGridItem)_fsView.SelectedItem;
            Directory createInDirectory;
            // if the selected item isn't a directory, set selected to the item's parent (the directory whom holds it)
            if (selectedItem.GetType() != typeof(DirectoryGridItem))
            {
                var parentDir = (DirectoryGridItem)selectedItem.Parent;
                createInDirectory = (Directory)parentDir.FilesystemObject;
                selectedItem = (FilesystemObjectGridItem)selectedItem.Parent;
            }
            else // the selected item is a directory
            {
                createInDirectory = (Directory)selectedItem.FilesystemObject;
            }
        }

        private void DeleteObject(object sender, EventArgs e)
        {
            if (_fs == null) return;
            var selectedItem = (FilesystemObjectGridItem)_fsView.SelectedItem;
            var parent = (FilesystemObjectGridItem)selectedItem.Parent;
            var fsObj = selectedItem.FilesystemObject;

            // remove from tree
            if (selectedItem.Parent == null) // probably root
            {
                MessageBox.Show("Filesystems are required to have a root directory", MessageBoxType.Error);
            }
            else
            {
                parent.Children.Remove(selectedItem);
                // remove from filesystem
                _fs.DeleteObject(fsObj);
                // reload
                _fsView.ReloadData();
            }
        }

        // CONTROL HANDLERS
        private void FsViewItemActivated(object sender, EventArgs e)
        {
            // reset text area
            _currentlyEditedFile = null;
            _textArea.Text = "";
            var selectedItem = (FilesystemObjectGridItem)_fsView.SelectedItem;
            if (selectedItem.GetType() != typeof(FileGridItem)) return;
            var file = (File)selectedItem.FilesystemObject;
            _currentlyEditedFile = file;
            _textArea.Text = Encoding.UTF8.GetString(file.Contents);
        }

        private void FsViewMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Buttons == MouseButtons.Alternate)
            {
                // open context menu
                _ctxMenu.Show(_fsView);
            }
        }

        private void TextChanged(object sender, EventArgs e)
        {
            // modify file contents
            _currentlyEditedFile?.ModifyContents(Encoding.UTF8.GetBytes(_textArea.Text), true);
        }
    }
}
