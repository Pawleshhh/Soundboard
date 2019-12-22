using wpf = System.Windows;

namespace ManiacSoundboard.ViewModel
{

    /// <summary>
    /// Specified implementation of <see cref="IMessageBoxService"/> for WPF framework.
    /// </summary>
    public class WpfAppMessageBox : IMessageBoxService
    {

        public WpfAppMessageBox()
        {

        }

        public WpfAppMessageBox(wpf.Window owner)
        {
            DefaultOwner = owner;
        }

        public wpf.Window DefaultOwner { get; set; }

        public void ShowMessageBox(string text, string caption, MessageBoxImage img)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (DefaultOwner != null)
                    wpf.MessageBox.Show(DefaultOwner, text, caption, (wpf.MessageBoxButton)MessageBoxButton.OK, (wpf.MessageBoxImage)img);
                else
                    wpf.MessageBox.Show(text, caption, (wpf.MessageBoxButton)MessageBoxButton.OK, (wpf.MessageBoxImage)img);
            });
        }

        public MessageBoxResult ShowMessageBoxDecision(string text, string caption, MessageBoxButton button, MessageBoxImage img)
        {
            MessageBoxResult result = default;
            App.Current.Dispatcher.Invoke(() =>
            {
                if (DefaultOwner != null)
                    result = (MessageBoxResult)wpf.MessageBox.Show(DefaultOwner, text, caption, (wpf.MessageBoxButton)button, (wpf.MessageBoxImage)img);
                else
                    result = (MessageBoxResult)wpf.MessageBox.Show(text, caption, (wpf.MessageBoxButton)button, (wpf.MessageBoxImage)img);
            });

            return result;
        }

        public void ShowMessageBox(wpf.Window owner, string text, string caption, MessageBoxImage img)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                wpf.MessageBox.Show(owner, text, caption, (wpf.MessageBoxButton)MessageBoxButton.OK, (wpf.MessageBoxImage)img);
            });
        }

        public MessageBoxResult ShowMessageBoxDecision(wpf.Window owner, string text, string caption, MessageBoxButton button, MessageBoxImage img)
        {
            MessageBoxResult result = default;
            App.Current.Dispatcher.Invoke(() =>
            {
                result = (MessageBoxResult)wpf.MessageBox.Show(owner, text, caption, (wpf.MessageBoxButton)button, (wpf.MessageBoxImage)img);
            });
            return result;
        }

        public void ShowMessageBox(string text, string caption, object img)
        {
             ShowMessageBox(text, caption, img);
        }

        public object ShowMessageBoxDecision(string text, string caption, object button, object img)
        {
            return ShowMessageBoxDecision(text, caption, button, img);
        }
    }
}
