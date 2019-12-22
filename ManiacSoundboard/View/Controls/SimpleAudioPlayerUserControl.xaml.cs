using System.Windows.Controls;
using System.Windows.Input;

namespace ManiacSoundboard
{
    /// <summary>
    /// Interaction logic for SimpleAudioPlayerUserControl.xaml
    /// </summary>
    public partial class SimpleAudioPlayerUserControl : BaseAudioPlayerUserControl
    {
        public SimpleAudioPlayerUserControl()
        {
            InitializeComponent();
        }

        private void keyBindingUserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void pathTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
