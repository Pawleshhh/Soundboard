
namespace ManiacSoundboard.ViewModel
{

    /// <summary>
    /// Defines interface of message box service.
    /// </summary>
    public interface IMessageBoxService
    {

        void ShowMessageBox(string text, string caption, MessageBoxImage img);

        MessageBoxResult ShowMessageBoxDecision(string text, string caption, MessageBoxButton button, MessageBoxImage img);

    }


}
