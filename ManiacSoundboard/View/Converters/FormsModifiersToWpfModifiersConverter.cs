using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using forms = System.Windows.Forms;

namespace ManiacSoundboard
{

    /// <summary>
    /// Converts forms modifiers to wpf modifiers and vice versa.
    /// </summary>
    public class FormsModifiersToWpfModifiersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is forms.Keys))
                throw new ArgumentException("value should be a type of System.Windows.Forms.Keys");

            forms.Keys key = (forms.Keys)value;

            return FormsWpfKeysHelper.WpfModifiersFromFormsModifiers(key);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ModifierKeys))
                throw new ArgumentException("value should be a type of System.Windows.Input.ModifierKeys");

            ModifierKeys key = (ModifierKeys)value;

            return FormsWpfKeysHelper.FormsModifiersFromWpfModifiers(key);
        }
    }
}
