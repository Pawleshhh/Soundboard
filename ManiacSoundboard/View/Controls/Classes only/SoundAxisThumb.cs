using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ManiacSoundboard
{

    /// <summary>
    /// Thumb used on sound axis.
    /// </summary>
    public class SoundAxisThumb : Thumb
    {

        public SoundAxisThumb()
        {
            MouseEnter += SoundAxisThumb_MouseEnter;
        }

        private void SoundAxisThumb_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
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
