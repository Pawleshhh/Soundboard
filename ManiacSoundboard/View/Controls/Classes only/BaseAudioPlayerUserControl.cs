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

    /// <summary>
    /// Base class for user controls that represent an audio player.
    /// </summary>
    public class BaseAudioPlayerUserControl : UserControl
    {

        public ICommand PlayStopCommand
        {
            get { return (ICommand)GetValue(PlayStopCommandProperty); }
            set { SetValue(PlayStopCommandProperty, value); }
        }

        public static readonly DependencyProperty PlayStopCommandProperty =
            DependencyProperty.Register("PlayStopCommand", typeof(ICommand), typeof(BaseAudioPlayerUserControl));

        public TimeSpan CurrentTime
        {
            get { return (TimeSpan)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(TimeSpan), typeof(BaseAudioPlayerUserControl));

        public TimeSpan TotalTime
        {
            get { return (TimeSpan)GetValue(TotalTimeProperty); }
            set { SetValue(TotalTimeProperty, value); }
        }

        public static readonly DependencyProperty TotalTimeProperty =
            DependencyProperty.Register("TotalTime", typeof(TimeSpan), typeof(BaseAudioPlayerUserControl));

        /// <summary>
        /// Contains bound modifiers. 
        /// </summary>
        public static readonly DependencyProperty ModifiersProperty =
            DependencyProperty.Register("Modifiers", typeof(ModifierKeys), typeof(BaseAudioPlayerUserControl));

        /// <summary>
        /// Gets bound modifiers.
        /// </summary>
        public ModifierKeys Modifiers
        {
            get { return (ModifierKeys)GetValue(ModifiersProperty); }
            set { SetValue(ModifiersProperty, value); }
        }

        /// <summary>
        /// Contains bound trigger key.
        /// </summary>
        public static readonly DependencyProperty TriggerKeyProperty =
            DependencyProperty.Register("TriggerKey", typeof(Key), typeof(BaseAudioPlayerUserControl));

        /// <summary>
        /// Gets bound trigger key.
        /// </summary>
        public Key TriggerKey
        {
            get { return (Key)GetValue(TriggerKeyProperty); }
            set { SetValue(TriggerKeyProperty, value); }
        }
    }
}
