using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ManiacSoundboard.Model
{
    public class KeyCombination : IEquatable<KeyCombination>
    {

        #region Constructors

        private KeyCombination()
        {

        }

        private KeyCombination(Keys triggerKey)
        {
            TriggerKey = triggerKey;
        }

        private KeyCombination(Keys triggerKey, IEnumerable<Keys> modifiers) : this(triggerKey)
        {
            _modifiers.UnionWith(modifiers.Where(k => k != Keys.None));
        }

        #endregion

        #region Private fields

        /// <summary>
        /// Hash set with <see cref="Keys"/> modifiers.
        /// </summary>
        private HashSet<Keys> _modifiers = new HashSet<Keys>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the trigger key.
        /// </summary>
        public Keys TriggerKey { get; private set; } = Keys.None;

        /// <summary>
        /// Gets the collection of modifiers.
        /// </summary>
        public IReadOnlyCollection<Keys> Modifiers => _modifiers;

        #endregion

        #region Methods

        /// <summary>
        /// Returns new <see cref="KeyCombination"/> keeping current trigger key and modifiers with <see cref="Keys.Control"/> modifier.
        /// </summary>
        public KeyCombination Control()
        {
            return With(Keys.Control);
        }

        /// <summary>
        /// Returns new <see cref="KeyCombination"/> keeping current trigger key and modifiers with <see cref="Keys.Alt"/> modifier.
        /// </summary>
        public KeyCombination Alt()
        {
            return With(Keys.Alt);
        }


        /// <summary>
        /// Returns new <see cref="KeyCombination"/> keeping current trigger key and modifiers with <see cref="Keys.Shift"/> modifier.
        /// </summary>
        public KeyCombination Shift()
        {
            return With(Keys.Shift);
        }


        /// <summary>
        /// Returns new <see cref="KeyCombination"/> keeping current trigger key and modifiers with new given modifier.
        /// </summary>
        /// <param name="modifier">Modifier to be added to the current trigger key and modifiers.</param>
        public KeyCombination With(Keys modifier)
        {
            if (modifier == Keys.None || _modifiers.Add(modifier) == false) return this;

            return new KeyCombination(TriggerKey, _modifiers);
        }

        public bool Equals(KeyCombination obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;

            return TriggerKey == obj.TriggerKey && Enumerable.SequenceEqual(_modifiers, obj._modifiers);
        }

        public override bool Equals(object obj)
        {
            if (obj is KeyCombination combination) return Equals(combination);

            return false;
        }

        public override int GetHashCode()
        {
            return TriggerKey.GetHashCode() * ToString().GetHashCode() * 7;
        }

        public override string ToString()
        {
            return string.Join("+", _modifiers.Concat(Enumerable.Repeat(TriggerKey, 1)));
        }

        #endregion

        #region Private methdos

        #endregion

        #region Static methods

        /// <summary>
        /// Returns new <see cref="KeyCombination"/> triggered by given <see cref="Keys"/>.
        /// </summary>
        /// <param name="key">Trigger key.</param>
        public static KeyCombination TriggeredBy(Keys key)
        {
            return new KeyCombination(key);
        }

        /// <summary>
        /// Returns new <see cref="KeyCombination"/> from given string. String must be in format: TriggerKey + Modifier1 + Modifier2 + ... + ModifierN.
        /// </summary>
        /// <param name="str">String that stores in proper format key combination.</param>
        public static KeyCombination FromString(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return new KeyCombination(Keys.None);

            //Get every key from str splited by '+'
            var parts = str
                .Split('+')
                .Select(p => Enum.Parse(typeof(Keys), p))
                .Cast<Keys>();
            //Convert it to Stack
            var stack = new Stack<Keys>(parts);
            //Pop the key which is the trigger key.
            var triggerKey = stack.Pop();
            //return new KeyCombination with trigger key and the rest keys as modifiers.
            return new KeyCombination(triggerKey, stack);
        }

        /// <summary>
        /// Return
        /// </summary>
        /// <param name="triggerKey"></param>
        /// <param name="modifiers"></param>
        /// <returns></returns>
        public static string StringFromKeys(Keys triggerKey, Keys modifiers)
        {
            if (triggerKey == Keys.None) return string.Empty;

            string key = triggerKey.ToString();

            if (modifiers == Keys.None) return key;

            string mods = modifiers.ToString();
            mods = mods.Replace(",", "+");

            return mods + "+" + key;
        }

        #endregion

    }
}
