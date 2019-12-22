using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ManiacSoundboard
{
    public class BaseAudioPlayerUserControl : UserControl
    {

        public ICommand PlayStopCommand
        {
            get { return (ICommand)GetValue(PlayStopCommandProperty); }
            set { SetValue(PlayStopCommandProperty, value); }
        }

        public static readonly DependencyProperty PlayStopCommandProperty =
            DependencyProperty.Register("PlayStopCommand", typeof(ICommand), typeof(BaseAudioPlayerUserControl));


    }
}
