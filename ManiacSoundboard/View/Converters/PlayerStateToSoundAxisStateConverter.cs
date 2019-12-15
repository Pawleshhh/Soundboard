using ManiacSoundboard.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ManiacSoundboard
{

    /// <summary>
    /// Converts PlayerState to SoundAxisState and vice versa.
    /// </summary>
    public class PlayerStateToSoundAxisStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var naudioState = (PlayerState)value;

            return (SoundAxisState)(int)naudioState;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var soundState = (SoundAxisState)value;

            return (PlayerState)(int)soundState;
        }
    }
}
