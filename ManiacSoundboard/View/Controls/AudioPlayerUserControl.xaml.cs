using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ManiacSoundboard
{
    /// <summary>
    /// Interaction logic for AudioPlayerUserControl.xaml
    /// </summary>
    public partial class AudioPlayerUserControl : BaseAudioPlayerUserControl
    {
        public AudioPlayerUserControl()
        {
            InitializeComponent();
        }

        public ICommand PlayPauseCommand
        {
            get { return (ICommand)GetValue(PlayPauseCommandProperty); }
            set { SetValue(PlayPauseCommandProperty, value); }
        }

        public static readonly DependencyProperty PlayPauseCommandProperty =
            DependencyProperty.Register("PlayPauseCommand", typeof(ICommand), typeof(AudioPlayerUserControl));

        public ICommand StopCommand
        {
            get { return (ICommand)GetValue(StopCommandProperty); }
            set { SetValue(StopCommandProperty, value); }
        }

        public static readonly DependencyProperty StopCommandProperty =
            DependencyProperty.Register("StopCommand", typeof(ICommand), typeof(AudioPlayerUserControl));

        ///// <summary>
        ///// Contains current time of an audio.
        ///// </summary>
        //public static readonly DependencyProperty CurrentTimeProperty =
        //    DependencyProperty.Register("CurrentTime", typeof(TimeSpan), typeof(AudioPlayerUserControl));

        ///// <summary>
        ///// Gets current time of an audio.
        ///// </summary>
        //public TimeSpan CurrentTime
        //{
        //    get => (TimeSpan)GetValue(CurrentTimeProperty);
        //    set => SetValue(CurrentTimeProperty, value);
        //}

        ///// <summary>
        ///// Contains path to an audio file.
        ///// </summary>
        //public static readonly DependencyProperty AudioFilePathProperty =
        //    DependencyProperty.Register("AudioFilePath", typeof(string), typeof(AudioPlayerUserControl));

        ///// <summary>
        ///// Gets an audio file path.
        ///// </summary>
        //public string AudioFilePath
        //{
        //    get => (string)GetValue(AudioFilePathProperty);
        //    set => SetValue(AudioFilePathProperty, value);
        //}

        ///// <summary>
        ///// Contains bound modifiers. 
        ///// </summary>
        //public static readonly DependencyProperty ModifiersProperty =
        //    DependencyProperty.Register("Modifiers", typeof(ModifierKeys), typeof(AudioPlayerUserControl));

        ///// <summary>
        ///// Gets bound modifiers.
        ///// </summary>
        //public ModifierKeys Modifiers
        //{
        //    get { return (ModifierKeys)GetValue(ModifiersProperty); }
        //    set { SetValue(ModifiersProperty, value); }
        //}

        ///// <summary>
        ///// Contains bound trigger key.
        ///// </summary>
        //public static readonly DependencyProperty TriggerKeyProperty =
        //    DependencyProperty.Register("TriggerKey", typeof(Key), typeof(AudioPlayerUserControl));

        ///// <summary>
        ///// Gets bound trigger key.
        ///// </summary>
        //public Key TriggerKey
        //{
        //    get { return (Key)GetValue(TriggerKeyProperty); }
        //    set { SetValue(TriggerKeyProperty, value); }
        //}

        private void PreventActivatingButtonsByEnterAndSpace(object sender, KeyEventArgs e)
        {
            //Handles the PreviewKeyDown event to prevent activating the play pause or stop button when pressing Enter or Space keys.
            if (e.Key == Key.Enter || e.Key == Key.Space)
                e.Handled = true;
        }


    }
}
