using System.Windows.Forms;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Sound with specified key binding.
    /// </summary>
    //public class Sound : IKeyBinding<IPlayer>
    //{

    //    #region Constructors

    //    public Sound()
    //    {

    //    }

    //    /// <summary>
    //    /// Initializes an instance of the <see cref="Sound"/> class.
    //    /// </summary>
    //    /// <param name="audioPath">Path to an audio file.</param>
    //    /// <exception cref="System.IO.FileNotFoundException"/>
    //    public Sound(string audioPath)
    //    {
    //        BoundData = new WavePlayer(audioPath);
    //    }

    //    /// <summary>
    //    /// Initializes an instance of the <see cref="Sound"/> class.
    //    /// </summary>
    //    /// <param name="audioPath">Path to an audio file.</param>
    //    /// <param name="key">Key that triggered the binding.</param>
    //    /// <param name="mods">Keys that had been pressed before the main key was pressed.</param>
    //    /// <exception cref="System.IO.FileNotFoundException"/>
    //    public Sound(string audioPath, Keys key, Keys mods = Keys.None) : this(audioPath)
    //    {
    //        _key = key;
    //        _mods = mods;

    //        Combination = KeyCombination.FromString(KeyCombination.StringFromKeys(Key, Modifiers));
    //    }

    //    #endregion

    //    #region Private fields

    //    #endregion

    //    #region Properties

    //    public IPlayer BoundData { get; private set; }

    //    object IKeyBinding.BoundData => BoundData;

    //    private Keys _key;

    //    public Keys Key
    //    {
    //        get => _key;
    //        set
    //        {
    //            _key = value;

    //            Combination = KeyCombination.FromString(KeyCombination.StringFromKeys(Key, Modifiers));
    //        }
    //    }

    //    private Keys _mods;

    //    public Keys Modifiers
    //    {
    //        get => _mods;
    //        set
    //        {
    //            _mods = value;

    //            Combination = KeyCombination.FromString(KeyCombination.StringFromKeys(Key, Modifiers));
    //        }
    //    }

    //    public string TagName { get; set; } = string.Empty;

    //    public KeyCombination Combination { get; private set; }

    //    #endregion

    //    #region Methods

    //    public bool Equals(IKeyBinding<IPlayer> other)
    //    {
    //        return other != null && Key.Equals(other.Key) && Modifiers.Equals(other.Modifiers) &&
    //               BoundData.Equals(other.BoundData) && TagName.Equals(other.TagName);
    //    }

    //    public bool Equals(IKeyBinding other)
    //    {
    //        if (other is IKeyBinding<IPlayer> player) return Equals(player);

    //        return false;
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (obj == null) return false;
    //        if (obj is IKeyBinding binding) return Equals(binding);

    //        return false;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return Key.GetHashCode() * BoundData.GetHashCode() * 7 + Modifiers.GetHashCode();
    //    }

    //    public override string ToString()
    //    {
    //        return string.IsNullOrWhiteSpace(TagName) ? BoundData.ToString() : TagName;
    //    }

    //    #endregion

    //    #region Private methods

    //    #endregion

    //}
}
