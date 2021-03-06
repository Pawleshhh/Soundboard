﻿using ManiacSoundboard.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ManiacSoundboard.ViewModel
{

    /// <summary>
    /// Extends <see cref="SoundViewModel"/> with data associated with keyboard keys.
    /// </summary>
    public sealed class SoundKeyboardViewModel : SoundViewModel, IEquatable<SoundKeyboardViewModel>
    {

        #region Constructors

        private SoundKeyboardViewModel() : base()
        {

        }

        public SoundKeyboardViewModel(IPlayer player, SoundboardViewModel soundboardViewModel) : base(player, soundboardViewModel)
        {
            Key = Keys.None;
            Modifiers = Keys.None;
        }

        #endregion

        #region Private fields

        #endregion

        #region Properties

        private Keys key;

        /// <summary>
        /// Gets or sets trigger key.
        /// </summary>
        public Keys Key
        {
            get => key;
            set
            {
                key = value;
                Combination = KeyCombination.FromString(KeyCombination.StringFromKeys(Key, Modifiers));
                OnPropertyChanged("Key", "Combination");
            }
        }

        private Keys modifiers;

        /// <summary>
        /// Gets or sets modifiers.
        /// </summary>
        public Keys Modifiers
        {
            get => modifiers;
            set
            {
                modifiers = value;
                Combination = KeyCombination.FromString(KeyCombination.StringFromKeys(Key, Modifiers));
                OnPropertyChanged("Modifiers", "Combination");
            }
        }

        /// <summary>
        /// Gets key combination from <see cref="Key"/> and <see cref="Modifiers"/> properties.
        /// </summary>
        public KeyCombination Combination { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Helper method to specify if this view model is associated with some data.
        /// </summary>
        public override bool IsAssociatedWith(object data)
        {
            return Combination != null && Combination.Equals(data);
        }

        public bool Equals(SoundKeyboardViewModel other)
        {
            return base.Equals(other) && other.Combination.Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (obj is SoundKeyboardViewModel soundViewModel) return Equals(soundViewModel);

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * 7 + Combination.GetHashCode() * 3;
        }

        public override Dictionary<string, string> LoadData()
        {
            var settings = base.LoadData();

            settings.Add("Key", Key.ToString());
            settings.Add("Modifiers", Modifiers.ToString());

            return settings;
        }

        #endregion

        #region Private methods

        #endregion

    }
}
