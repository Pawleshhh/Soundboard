using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ManiacSoundboard.Model
{
    public class KeyCombination : IEquatable<KeyCombination>, IXmlSerializable
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

        private HashSet<Keys> _modifiers = new HashSet<Keys>();

        #endregion

        #region Properties

        public Keys TriggerKey { get; private set; } = Keys.None;

        public IReadOnlyCollection<Keys> Modifiers => _modifiers;

        #endregion

        #region Methods

        public KeyCombination Control()
        {
            return With(Keys.Control);
        }

        public KeyCombination Alt()
        {
            return With(Keys.Alt);
        }

        public KeyCombination Shift()
        {
            return With(Keys.Shift);
        }

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

        public static KeyCombination TriggeredBy(Keys key)
        {
            return new KeyCombination(key);
        }

        public static KeyCombination FromString(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return new KeyCombination(Keys.None);

            var parts = str
                .Split('+')
                .Select(p => Enum.Parse(typeof(Keys), p))
                .Cast<Keys>();
            var stack = new Stack<Keys>(parts);
            var triggerKey = stack.Pop();
            return new KeyCombination(triggerKey, stack);
        }

        public static string StringFromKeys(Keys triggerKey, Keys modifiers)
        {
            if (triggerKey == Keys.None) return string.Empty;

            string key = triggerKey.ToString();

            if (modifiers == Keys.None) return key;

            string mods = modifiers.ToString();
            mods = mods.Replace(",", "+");

            return mods + "+" + key;
        }

        private static Keys NormalizeFormsKey(Keys key)
        {
            if (key == Keys.LControlKey || key == Keys.RControlKey) return Keys.Control;
            if (key == Keys.LMenu || key == Keys.RMenu) return Keys.Alt;
            if (key == Keys.LShiftKey || key == Keys.RShiftKey) return Keys.Shift;

            return key;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();

            string str = reader.ReadElementContentAsString("KeyCombination", "");

            var parts = str
                .Split('+')
                .Select(p => Enum.Parse(typeof(Keys), p))
                .Cast<Keys>();
            var stack = new Stack<Keys>(parts);
            var triggerKey = stack.Pop();

            _modifiers = new HashSet<Keys>(stack);
            TriggerKey = triggerKey;

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("KeyCombination", ToString());
        }

        #endregion

    }
}
