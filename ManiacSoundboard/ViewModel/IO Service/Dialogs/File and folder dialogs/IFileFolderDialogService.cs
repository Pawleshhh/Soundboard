namespace ManiacSoundboard.ViewModel
{
    public interface IFileFolderDialogService
    {
        (string Path, DialogResult Result) OpenSingleFile();

        (string[] Paths, DialogResult Result) OpenMultipleFiles();

        (string Path, DialogResult Result) OpenSingleFolder();

    }
}
