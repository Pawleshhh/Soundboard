using ManiacSoundboard.Model;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace ManiacSoundboard
{
    class KeyCombinationModifiersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Key))
                throw new ArgumentException("The target type has to be System.Windows.Input.Key type.");

            KeyCombination combination = (KeyCombination)value;

            Keys modifiers = Keys.None;

            foreach (var key in combination.Modifiers)
                modifiers = modifiers | key;

            return modifiers;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Keys modifiers = FormsWpfKeysHelper.FormsModifiersFromWpfModifiers((ModifierKeys)value);

            return null;
        }
    }
}
