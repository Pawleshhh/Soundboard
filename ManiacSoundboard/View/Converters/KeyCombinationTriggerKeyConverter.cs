using ManiacSoundboard.Model;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace ManiacSoundboard
{
    public class KeyCombinationTriggerKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            KeyCombination combination = (KeyCombination)value;

            return combination.TriggerKey;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Keys key = FormsWpfKeysHelper.FormsKeyFromWpfKey((Key)value);

            return key;
        }
    }
}
