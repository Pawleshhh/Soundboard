
namespace ManiacSoundboard.ViewModel
{
    public interface IMessageBoxService
    {

        void ShowMessageBox(string text, string caption, MessageBoxImage img);

        MessageBoxResult ShowMessageBoxDecision(string text, string caption, MessageBoxButton button, MessageBoxImage img);

    }


}
