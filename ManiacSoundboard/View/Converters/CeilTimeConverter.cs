using System;
using System.Globalization;
using System.Windows.Data;

namespace ManiacSoundboard
{
    /// <summary>
    /// Ceils time and not vice versa.
    /// </summary>
    public class CeilTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = (TimeSpan)value;

            if (time < TimeSpan.FromSeconds(1))
                return TimeSpan.FromSeconds(1);
            else
                return time;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
