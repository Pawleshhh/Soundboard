using System.Windows.Forms;

namespace ManiacSoundboard.ViewModel
{

    /// <summary>
    /// Specified implementation of <see cref="IFileFolderDialogService"/> for WPF framework.
    /// </summary>
    public class WpfAppFileFolderDialog : IFileFolderDialogService
    {

        #region Private fields

        #endregion

        #region Properties

        public string Filter { get; set; } = "All files (*.*)|*.*";

        public string InitlialDirectory { get; set; } = "c:\\";

        public bool RestoreDirectory { get; set; } = true;

        public bool ShowNewFolderButton { get; set; }

        #endregion

        #region Methods

        public (string Path, DialogResult Result) OpenSingleFile()
        {
            using(var openFileDialog = new OpenFileDialog())
            {
                _InitializeOpenFileDialog(openFileDialog);

                if ((int)openFileDialog.ShowDialog() == (int)DialogResult.OK)
                {
                    return (openFileDialog.FileName, DialogResult.OK);
                }

                return (string.Empty, DialogResult.Cancel);
            }
        }

        public (string[] Paths, DialogResult Result) OpenMultipleFiles()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                _InitializeOpenFileDialog(openFileDialog);
                openFileDialog.Multiselect = true;

                if ((int)openFileDialog.ShowDialog() == (int)DialogResult.OK)
                {
                    return (openFileDialog.FileNames, DialogResult.OK);
                }

                return (new string[] { }, DialogResult.Cancel);
            }
        }

        public (string Path, DialogResult Result) OpenSingleFolder()
        {
            using (var openFolderDialog = new FolderBrowserDialog())
            {
                openFolderDialog.ShowNewFolderButton = ShowNewFolderButton;

                if((int)openFolderDialog.ShowDialog() == (int)DialogResult.OK)
                {
                    return (openFolderDialog.SelectedPath, DialogResult.OK);
                }

                return (string.Empty, DialogResult.Cancel);
            }
        }

        #endregion

        #region Private methods

        private void _InitializeOpenFileDialog(OpenFileDialog openFileDialog)
        {
            openFileDialog.Filter = Filter;
            //openFileDialog.InitialDirectory = InitlialDirectory;
            openFileDialog.RestoreDirectory = RestoreDirectory;
        }

        #endregion

    }

}
