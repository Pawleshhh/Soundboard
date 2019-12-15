using System;
using System.Windows.Forms;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Represents the interface of all key bindings.
    /// </summary>
    public interface IKeyBinding : IEquatable<IKeyBinding>
    {

        /// <summary>
        /// Gets or sets main key of the binding.
        /// </summary>
        Keys Key { get; set; }

        /// <summary>
        /// Gets or sets modifiers for the binding.
        /// </summary>
        Keys Modifiers { get; set; }

        /// <summary>
        /// Gets or sets tag name for the binding.
        /// </summary>
        string TagName { get; set; }

        /// <summary>
        /// Gets the data that is bound to the key.
        /// </summary>
        object BoundData { get; }

        /// <summary>
        /// Gets the combination from the bound keys.
        /// </summary>
        /// <returns></returns>
        KeyCombination Combination { get; }

    }

    /// <summary>
    /// Represents the interface of all key bindings.
    /// </summary>
    /// <typeparam name="T">Type of the data that is bound with the keys.</typeparam>
    public interface IKeyBinding<T> : IKeyBinding, IEquatable<IKeyBinding<T>>
    {

        /// <summary>
        /// Gets the data that is bound to the key.
        /// </summary>
        new T BoundData { get; }

    }
}
