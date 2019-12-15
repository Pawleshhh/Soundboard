using System;
using System.Globalization;
using System.Windows.Data;

namespace ManiacSoundboard
{

    /// <summary>
    /// Rounds Volume.
    /// </summary>
    public class RoundVolumeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float fvalue = (float)value;

            return Math.Round((double)fvalue, 2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
