using System;
using System.Globalization;
using System.Windows.Data;

namespace ManiacSoundboard
{

    /// <summary>
    /// Converts TimeSpan to double in regard of parameter which is TimeUnits enum and vice versa.
    /// </summary>
    public class TimeSpanToDoubleConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeSpan = (TimeSpan)value;
            TimeUnits unit = (TimeUnits)parameter;

            if (unit == TimeUnits.Seconds)
                return timeSpan.TotalSeconds;
            if (unit == TimeUnits.Minutes)
                return timeSpan.TotalMinutes;

            return timeSpan.TotalMilliseconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = (double)value;
            TimeUnits unit = (TimeUnits)parameter;

            if (unit == TimeUnits.Seconds)
                return TimeSpan.FromSeconds(time);
            if (unit == TimeUnits.Minutes)
                return TimeSpan.FromMinutes(time);

            return TimeSpan.FromMilliseconds(time);
        }

    }

    public enum TimeUnits
    {
        Milliseconds, Seconds, Minutes
    }
}
