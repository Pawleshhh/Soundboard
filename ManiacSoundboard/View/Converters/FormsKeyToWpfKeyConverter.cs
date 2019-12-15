using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using forms = System.Windows.Forms;

namespace ManiacSoundboard
{

    /// <summary>
    /// Converts forms key to wpf key and vice versa.
    /// </summary>
    public class FormsKeyToWpfKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is forms.Keys))
                throw new ArgumentException("value should be a type of System.Windows.Forms.Keys");

            forms.Keys key = (forms.Keys)value;

            return FormsWpfKeysHelper.WpfKeyFromFormsKey(key);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Key))
                throw new ArgumentException("value should be a type of System.Windows.Input.Key");

            Key key = (Key)value;

            return FormsWpfKeysHelper.FormsKeyFromWpfKey(key);
        }
    }
}
