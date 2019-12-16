using Gma.System.MouseKeyHook;
using System;
using System.Windows.Forms;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Provides functions to help with using <see cref="KeyCombination"/> type and <see cref="Keys"/> type.
    /// </summary>
    public static class CombinationHelper
    {

        public static string GetStringCombination(Keys triggerKey, Keys modifiers)
        {
            string triggerKeystr = triggerKey.ToString();

            if (modifiers == Keys.None) return triggerKeystr;

            string mods = modifiers.ToString();

            mods = mods.Replace(",", "+");

            return $"{mods}+{triggerKeystr}";
        }

        //public static Combination GetCombination(Keys triggerKey, Keys modifiers)
        //{
        //    return Combination.FromString(GetStringCombination(triggerKey, modifiers));
        //}

        public static Combination GetCombination(Keys triggerKey, Keys modifiers)
        {
            if (triggerKey == Keys.None || modifiers == Keys.None) return Combination.TriggeredBy(triggerKey);

            Combination combination = Combination.TriggeredBy(triggerKey);

            foreach (Enum value in Enum.GetValues(modifiers.GetType()))
                if ((Keys)value != Keys.None && modifiers.HasFlag(value))
                    combination = combination.With((Keys)value);

            return combination;
        }

        public static Keys GetModifiers(KeyCombination combination)
        {
            Keys mods = Keys.None;
            foreach(var key in combination.Modifiers)
            {
                mods = mods | key;
            }

            return mods;
        }

    }
}
