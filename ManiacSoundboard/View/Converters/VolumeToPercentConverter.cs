using System;
using System.Globalization;
using System.Windows.Data;

namespace ManiacSoundboard
{

    /// <summary>
    /// Converts double to percents and not vice versa.
    /// </summary>
    public class DoubleToPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float number = (float)value;

            return number * 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
