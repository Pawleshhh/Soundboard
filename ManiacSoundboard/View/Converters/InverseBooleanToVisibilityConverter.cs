using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ManiacSoundboard
{

    /// <summary>
    /// Inverses (negates) converting boolean value to visibility and vice versa.
    /// </summary>
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {

        private BooleanToVisibilityConverter _bool2ToVisib = new BooleanToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be visibility");

            bool? _value = (bool?)value;
            if (!_value.HasValue) return Visibility.Hidden;
            if (_value.Value) return Visibility.Collapsed;
            else return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool?))
                throw new InvalidOperationException("The target must be boolean");

            if ((Visibility)value == Visibility.Visible) return false;

            return true;
        }
    }
}
