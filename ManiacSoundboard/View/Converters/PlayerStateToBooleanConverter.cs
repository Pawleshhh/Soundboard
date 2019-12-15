using ManiacSoundboard.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ManiacSoundboard
{

    /// <summary>
    /// Converts PlayerState to bool (playing == true) and vice versa.
    /// </summary>
    public class PlayerStateToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is PlayerState)
                    return (PlayerState)value == PlayerState.Playing ? true : false;

                return (SoundAxisState)value == SoundAxisState.Playing ? true : false;
            }
            catch
            {
                throw new InvalidOperationException("Value is not PlaybackState enum or SoundAxisState");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? bValue = (bool?)value;

            if (bValue.HasValue)
            {
                if (bValue.Value) return PlayerState.Playing;
                else return PlayerState.Stopped;
            }
            else
                return PlayerState.Paused;
        }
    }
}
