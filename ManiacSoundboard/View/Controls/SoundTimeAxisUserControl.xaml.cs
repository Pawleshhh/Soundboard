using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ManiacSoundboard
{
    /// <summary>
    /// Interaction logic for SoundTimeAxisUserControl.xaml
    /// </summary>
    public partial class SoundTimeAxisUserControl : UserControl
    {

        public SoundTimeAxisUserControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(TimeSpan), typeof(SoundTimeAxisUserControl));

        /// <summary>
        /// Gets or sets current time of played audio.
        /// </summary>
        public TimeSpan CurrentTime
        {
            get { return (TimeSpan)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        public static readonly DependencyProperty TotalTimeProperty =
            DependencyProperty.Register("TotalTime", typeof(TimeSpan), typeof(SoundTimeAxisUserControl));

        /// <summary>
        /// Gets or sets total time of the audio.
        /// </summary>
        public TimeSpan TotalTime
        {
            get { return (TimeSpan)GetValue(TotalTimeProperty); }
            set { SetValue(TotalTimeProperty, value); }
        }

        public static readonly DependencyProperty IsTimeVisibleProperty =
            DependencyProperty.Register("IsTimeVisible", typeof(bool), typeof(SoundTimeAxisUserControl), new PropertyMetadata(true, OnIsTimeVisibleChanged));

        /// <summary>
        /// Gets or sets whether the time is shown on the control or not.
        /// </summary>
        public bool IsTimeVisible
        {
            get { return (bool)GetValue(IsTimeVisibleProperty); }
            set { SetValue(IsTimeVisibleProperty, value); }
        }

        private static void OnIsTimeVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var axis = (SoundTimeAxisUserControl)d;

            if (axis.slider == null) return;

            bool value = (bool)e.NewValue;
            if (value)
                Grid.SetColumnSpan(axis.slider, 1);
            else
                Grid.SetColumnSpan(axis.slider, 2);
        }

        public static readonly DependencyProperty SoundStateProperty =
            DependencyProperty.Register("SoundState", typeof(SoundAxisState), typeof(SoundTimeAxisUserControl));

        /// <summary>
        /// Gets or sets the state of the audio.
        /// </summary>
        public SoundAxisState SoundState
        {
            get { return (SoundAxisState)GetValue(SoundStateProperty); }
            set { SetValue(SoundStateProperty, value); }
        }

        public static readonly DependencyProperty PauseCommandProperty =
            DependencyProperty.Register("PauseCommand", typeof(ICommand), typeof(SoundTimeAxisUserControl));

        /// <summary>
        /// Gets or sets the pause command.
        /// </summary>
        public ICommand PauseCommand
        {
            get { return (ICommand)GetValue(PauseCommandProperty); }
            set { SetValue(PauseCommandProperty, value); }
        }

        public static readonly DependencyProperty StartCommandProperty =
            DependencyProperty.Register("StartCommand", typeof(ICommand), typeof(SoundTimeAxisUserControl));

        /// <summary>
        /// Gets or sets the start command.
        /// </summary>
        public ICommand StartCommand
        {
            get { return (ICommand)GetValue(StartCommandProperty); }
            set { SetValue(StartCommandProperty, value); }
        }

        public static readonly DependencyProperty StopCommandProperty =
            DependencyProperty.Register("StopCommand", typeof(ICommand), typeof(SoundTimeAxisUserControl));

        /// <summary>
        /// Gets or sets the stop command.
        /// </summary>
        public ICommand StopCommand
        {
            get { return (ICommand)GetValue(StopCommandProperty); }
            set { SetValue(StopCommandProperty, value); }
        }

        private void Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            //BindingOperations.GetBindingExpression(this, SoundStateProperty)?.UpdateTarget();

            if (SoundState == SoundAxisState.Playing)
                PauseCommand?.Execute(null);
        }

        private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (SoundState == SoundAxisState.Paused)
                StartCommand?.Execute(null);
        }

        private void Thumb_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.MouseDevice.Captured == null)
            {
                // the left button is pressed on mouse enter
                // but the mouse isn't captured, so the thumb
                // must have been moved under the mouse in response
                // to a click on the track.
                // Generate a MouseLeftButtonDown event.

                MouseButtonEventArgs args = new MouseButtonEventArgs(
                    e.MouseDevice, e.Timestamp, MouseButton.Left);

                args.RoutedEvent = MouseLeftButtonDownEvent;

                (sender as Thumb).RaiseEvent(args);
            }
        }
    }

    
}
