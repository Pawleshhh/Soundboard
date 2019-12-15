using System;
using wpf = System.Windows.Input;
using forms = System.Windows.Forms;

namespace ManiacSoundboard
{

    /// <summary>
    /// Provides methods for easy use between WPF and Forms key enum types.
    /// </summary>
    public static class FormsWpfKeysHelper
    {

        /// <summary>
        /// Describes key combination from WPF key enum types.
        /// </summary>
        /// <param name="triggerKey">Trigger key.</param>
        /// <param name="modifiers">Modifiers combined with the trigger key.</param>
        /// <returns>Retruns descripton of key combination.</returns>
        public static string DescribeWpfBinding(wpf.Key triggerKey, wpf.ModifierKeys modifiers)
        {
            string keyName = GetFriendlyName(triggerKey);

            if (modifiers != wpf.ModifierKeys.None)
            {
                string[] sEnums = modifiers.ToString().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                string result = string.Empty;
                foreach (string en in sEnums)
                {
                    result += en + "+";
                }

                result += keyName;

                return result;
            }

            return keyName;
        }

        /// <summary>
        /// Describes pressed keys from WPF <see cref="wpf.KeyEventArgs"/>.
        /// </summary>
        /// <param name="e">WPF <see cref="wpf.KeyEventArgs"/></param>
        /// <returns>Retruns descripton of key combination.</returns>
        public static string DescribeWpfBinding(wpf.KeyEventArgs e)
        {
            var triggerKeyWithMods = WpfKeysFromWpfKeyEventArgs(e);

            return DescribeWpfBinding(triggerKeyWithMods.TriggerKey, triggerKeyWithMods.ModifierKeys);
        }

        /// <summary>
        /// Removes WPF <see cref="wpf.Key"/> enum value that is related to <see cref="wpf.ModifierKeys"/>
        /// value (e.g. if modifierKeys contains Alt and Control and if key contains LeftAlt or Right Alt then Alt value will be removed from modifierKeys).
        /// </summary>
        /// <param name="modifierKeys">Modifier keys with value to remove.</param>
        /// <param name="key">Key value to remove from modifiers.</param>
        /// <returns>Returns <see cref="wpf.ModifierKeys"/> without specified related value in <see cref="wpf.Key"/>.</returns>
        public static wpf.ModifierKeys RemoveWpfKeyFromModifierKeys(wpf.ModifierKeys modifierKeys, wpf.Key key)
        {
            if (key == wpf.Key.LeftAlt || key == wpf.Key.RightAlt)
                return modifierKeys & ~wpf.ModifierKeys.Alt;

            if (key == wpf.Key.LeftCtrl || key == wpf.Key.RightCtrl)
                return modifierKeys & ~wpf.ModifierKeys.Control;

            if (key == wpf.Key.LeftShift || key == wpf.Key.RightShift)
                return modifierKeys & ~wpf.ModifierKeys.Shift;

            return modifierKeys;
        }

        /// <summary>
        /// Returns tuple with trigger key and modifier keys from <see cref="wpf.KeyEventArgs"/>.
        /// </summary>
        /// <param name="e"><see cref="wpf.KeyEventArgs"/> with information about pressed key and modifiers (system keys).</param>
        public static (wpf.Key TriggerKey, wpf.ModifierKeys ModifierKeys) WpfKeysFromWpfKeyEventArgs(wpf.KeyEventArgs e)
        {
            //System key pressed.
            if (e.Key == wpf.Key.System)
            {
                //SystemKey is now the trigger key.
                if (e.SystemKey != wpf.Key.None)
                {
                    //SystemKey pressed with modifiers
                    if (e.KeyboardDevice.Modifiers != wpf.ModifierKeys.None)
                    {
                        wpf.ModifierKeys modifiers = e.KeyboardDevice.Modifiers;
                        wpf.Key key = e.SystemKey;
                        modifiers = RemoveWpfKeyFromModifierKeys(modifiers, key);
                        return (key, modifiers);
                    }
                    //SystemKey is only key pressed (e.g. LeftControl, RightAlt, etc.)
                    else
                    {
                        return (e.SystemKey, wpf.ModifierKeys.None);
                    }
                }
            }
            //Trigger key is a modifier.
            else if (e.KeyboardDevice.Modifiers != wpf.ModifierKeys.None && IsModifier(e.Key))
            {
                //Only modifier keys pressed
                if (e.SystemKey == wpf.Key.None)
                {
                    wpf.ModifierKeys modifiers = e.KeyboardDevice.Modifiers;
                    wpf.Key key = e.Key;
                    modifiers = RemoveWpfKeyFromModifierKeys(modifiers, key);
                    return (key, modifiers);
                }
            }

            return (e.Key, e.KeyboardDevice.Modifiers);
        }

        /// <summary>
        /// Returns trigger key and modifiers from WPF <see cref="wpf.KeyEventArgs"/> as Forms' <see cref="forms.Keys"/> enum type.
        /// </summary>
        /// <param name="e">WPF <see cref="wpf.KeyEventArgs"/> event arguments.</param>
        public static (forms.Keys TriggerKey, forms.Keys ModifierKeys) FormsKeysFromWpfKeyEventArgs(wpf.KeyEventArgs e)
        {
            var triggerKeyWithMods = WpfKeysFromWpfKeyEventArgs(e);
            forms.Keys key = FormsKeyFromWpfKey(triggerKeyWithMods.TriggerKey);
            forms.Keys modifiers = FormsModifiersFromWpfModifiers(triggerKeyWithMods.ModifierKeys);

            return (key, modifiers);
        }

        /// <summary>
        /// Returns Forms modifiers as <see cref="forms.Keys"/> from WPF <see cref="wpf.ModifierKeys"/>.
        /// </summary>
        /// <param name="mKeys">WPF <see cref="wpf.ModifierKeys"/> which is about to converted to Forms <see cref="forms.Keys"/>.</param>

        public static forms.Keys FormsModifiersFromWpfModifiers(wpf.ModifierKeys mKeys)
        {
            forms.Keys fKey = forms.Keys.None;
            if (mKeys.HasFlag(wpf.ModifierKeys.Control)) fKey = fKey | forms.Keys.Control;
            if (mKeys.HasFlag(wpf.ModifierKeys.Alt)) fKey = fKey | forms.Keys.Alt;
            if (mKeys.HasFlag(wpf.ModifierKeys.Shift)) fKey = fKey | forms.Keys.Shift;

            return fKey;
        }

        /// <summary>
        /// Returns WPF modifiers as <see cref="wpf.ModifierKeys"/> from Forms <see cref="forms.Keys"/>.
        /// </summary>
        /// <param name="mKeys">Forms <see cref="forms.Keys"/> which is about to converted to WPF <see cref="wpf.ModifierKeys"/>.</param>
        public static wpf.ModifierKeys WpfModifiersFromFormsModifiers(forms.Keys mKeys)
        {
            wpf.ModifierKeys wKey = wpf.ModifierKeys.None;
            if (mKeys.HasFlag(forms.Keys.Control)) wKey = wKey | wpf.ModifierKeys.Control;
            if (mKeys.HasFlag(forms.Keys.Alt)) wKey = wKey | wpf.ModifierKeys.Alt;
            if (mKeys.HasFlag(forms.Keys.Shift)) wKey = wKey | wpf.ModifierKeys.Shift;

            return wKey;
        }

        /// <summary>
        /// Returns Forms key as <see cref="forms.Keys"/> from WPF <see cref="wpf.Key"/>.
        /// </summary>
        /// <param name="key">WPF <see cref="wpf.Key"/> which is about to converted to Forms <see cref="forms.Keys"/>.</param>
        /// <returns></returns>
        public static forms.Keys FormsKeyFromWpfKey(wpf.Key key)
        {
            return (forms.Keys)wpf.KeyInterop.VirtualKeyFromKey(key);
        }

        /// <summary>
        /// Returns WPF key as <see cref="wpf.Key"/> from Forms <see cref="forms.Keys"/>.
        /// </summary>
        /// <param name="key">Forms <see cref="forms.Keys"/> which is about to converted to WPF <see cref="wpf.Key"/>.</param>
        /// <returns></returns>
        public static wpf.Key WpfKeyFromFormsKey(forms.Keys key)
        {
            return wpf.KeyInterop.KeyFromVirtualKey((int)key);
        }

        /// <summary>
        /// Returns friendly name of given Forms <see cref="forms.Keys"/>.
        /// </summary>
        /// <param name="key">Forms <see cref="forms.Keys"/>.</param>
        public static string GetFriendlyName(forms.Keys key)
        {
            if (!Enum.IsDefined(typeof(forms.Keys), key)) return "None";

            int value = (int)key;

            if (48 < value && value < 58)
            {
                return (value - 48).ToString();
            }

            switch (value)
            {
                case 48: return "0";
                case 13: return "Enter";
                case 20: return "CapsLock";
                case 33: return "PageUp";
                case 34: return "PageDown";
                case 44: return "PrintScreen";
                case 192: return "~";
                case 219: return "[";
                case 220: return @"\";
                case 221: return "]";
                case 222: return "\"";
                case 226: return @"\";
                case 191: return "?";
                case 107: return "NumPadPlus";
                case 8: return "Backspace";
                case 110: return "NumPadPeriod";
                case 111: return "NumPadSlash";
                case 40: return "DownArrow";
                case 162: return "LeftCtrl";
                case 37: return "LeftArrow";
                case 164: return "LeftAlt";
                case 160: return "LeftShift";
                case 38: return "UpArrow";
                case 39: return "RightArrow";
                case 18: return "Alt";
                case 186: return ";";
                case 189: return "-";
                case 187: return "+";
                case 188: return ",";
                case 190: return ".";
                case 109: return "NumPadMinus";
                case 106: return "NumPadStar";
                case 165: return "RightAlt";
                case 163: return "RightCtrl";
                case 161: return "RightShift";
            }

            return key.ToString();
        }

        /// <summary>
        /// Returns friendly name of given WPF <see cref="wpf.Key"/>.
        /// </summary>
        /// <param name="key">Forms <see cref="forms.Keys"/>.</param>
        public static string GetFriendlyName(wpf.Key key)
        {
            if (!Enum.IsDefined(typeof(wpf.Key), key)) return "None";
            if (key == wpf.Key.None) return "None";

            int value = (int)key;

            if (34 < value && value < 44)
            {
                return (value - 34).ToString();
            }

            switch (value)
            {
                case 34: return "0";
                case 89: return "NumPadSlash";
                case 84: return "NumPadStar";
                case 87: return "NumPadMinus";
                case 85: return "NumPadPlus";
                case 88: return "NumPadPeriod";
                case 146: return "~";
                case 143: return "-";
                case 141: return "+";
                case 2: return "Backspace";
                case 6: return "Enter";
                case 149: return "[";
                case 151: return "]";
                case 8: return "CapsLock";
                case 140: return ";";
                case 152: return "\"";
                case 150: return @"\";
                case 142: return ",";
                case 144: return ".";
                case 145: return "?";
                case 23: return "LeftArrow";
                case 24: return "UpArrow";
                case 25: return "RightArrow";
                case 26: return "DownArrow";
            }

            return key.ToString();
        }

        /// <summary>
        /// Checks if given WPF <see cref="wpf.Key"/> is a modifier.
        /// </summary>
        /// <param name="key">WPF <see cref="wpf.Key"/> to be checked.</param>
        /// <returns>Returns true if given key is a modifier; otherwise returns false.</returns>
        public static bool IsModifier(wpf.Key key)
        {
            if (key == wpf.Key.LeftShift || key == wpf.Key.RightShift ||
               key == wpf.Key.LeftCtrl || key == wpf.Key.RightCtrl ||
               key == wpf.Key.LeftAlt || key == wpf.Key.RightAlt)
                return true;

            return false;
        }

    }
}