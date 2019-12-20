namespace ManiacSoundboard.ViewModel
{

    /// <summary>
    /// Defines interface of file folder dialog service.
    /// </summary>
    public interface IFileFolderDialogService
    {
        (string Path, DialogResult Result) OpenSingleFile();

        (string[] Paths, DialogResult Result) OpenMultipleFiles();

        (string Path, DialogResult Result) OpenSingleFolder();

    }
}
