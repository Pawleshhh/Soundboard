using ManiacSoundboard.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;

namespace ManiacSoundboard.ViewModel
{
    public sealed class SoundboardKeyboardViewModel : SoundboardViewModel
    {

        #region Constructors

        public SoundboardKeyboardViewModel() : base()
        {
            Initialize();
        }

        public SoundboardKeyboardViewModel(Soundboard soundboard, IMessageBoxService msgBoxServ, IFileFolderDialogService fileFolderServ) : base(soundboard, msgBoxServ, fileFolderServ)
        {
            Initialize();
        }

        private void Initialize()
        {
            _globalKeyEvents = new GlobalKeyEvents();
            _globalKeyEvents.KeyDown += _globalKeyEvents_KeyDown;
            HandlesKeyEvents = true;
            PlayPausedKey = KeyCombination.TriggeredBy(Keys.F1);
            PauseKey = KeyCombination.TriggeredBy(Keys.F2);
            StopKey = KeyCombination.TriggeredBy(Keys.F3);
            MuteVolumeKey = KeyCombination.TriggeredBy(Keys.M).Control();
            IncreaseVolumeKey = KeyCombination.TriggeredBy(Keys.Oemplus);
            DecreaseVolumeKey = KeyCombination.TriggeredBy(Keys.OemMinus);
        }

        #endregion

        #region Private fields

        private GlobalKeyEvents _globalKeyEvents;

        #endregion

        #region Properties

        private bool handlesKeyEvents;

        public bool HandlesKeyEvents
        {
            get => handlesKeyEvents;
            set
            {
                handlesKeyEvents = value;
                OnPropertyChanged("HandlesKeyEvents");
            }
        }

        private KeyCombination playPausedKey;

        public KeyCombination PlayPausedKey
        {
            get => playPausedKey;
            set
            {
                playPausedKey = value;
                OnPropertyChanged("PlayPausedKey");
            }
        }

        private KeyCombination pauseKey;

        public KeyCombination PauseKey
        {
            get => pauseKey;
            set
            {
                pauseKey = value;
                OnPropertyChanged("PauseKey");
            }
        }

        private KeyCombination stopKey;

        public KeyCombination StopKey
        {
            get => stopKey;
            set
            {
                stopKey = value;
                OnPropertyChanged("StopKey");
            }
        }

        private KeyCombination muteVolumeKey;

        public KeyCombination MuteVolumeKey
        {
            get => muteVolumeKey;
            set
            {
                muteVolumeKey = value;
                OnPropertyChanged("MuteVolumeKey");
            }
        }

        private KeyCombination increaseVolumeKey;

        public KeyCombination IncreaseVolumeKey
        {
            get => increaseVolumeKey;
            set
            {
                increaseVolumeKey = value;
                OnPropertyChanged("IncreaseVolumeKey");
            }
        }

        private KeyCombination decreaseVolumeKey;

        public KeyCombination DecreaseVolumeKey
        {
            get => decreaseVolumeKey;
            set
            {
                decreaseVolumeKey = value;
                OnPropertyChanged("DecreaseVolumeKey");
            }
        }
        #endregion

        #region Methods

        public override SoundViewModel GetSoundViewModel(IPlayer player)
        {
            return new SoundKeyboardViewModel(player, this);
        }

        protected override SoundViewModel GetSoundViewModelFromXml(XmlReader r)
        {
            var soundVM = (SoundKeyboardViewModel)base.GetSoundViewModelFromXml(r);

            if (soundVM == null) return null;

            soundVM.Key = (Keys)Enum.Parse(typeof(Keys), r.ReadElementContentAsString(nameof(SoundKeyboardViewModel.Key), ""));
            soundVM.Modifiers = (Keys)Enum.Parse(typeof(Keys), r.ReadElementContentAsString(nameof(SoundKeyboardViewModel.Modifiers), ""));

            return soundVM;
        }

        protected override void ReadXmlSettings(XmlReader r)
        {
            base.ReadXmlSettings(r);

            //<KeyBindings>
            r.ReadStartElement();
            HandlesKeyEvents = r.ReadElementContentAsBoolean(nameof(HandlesKeyEvents), "");
            PlayPausedKey = KeyCombination.FromString(r.ReadElementContentAsString(nameof(PlayPausedKey), ""));
            PauseKey = KeyCombination.FromString(r.ReadElementContentAsString(nameof(PauseKey), ""));
            StopKey = KeyCombination.FromString(r.ReadElementContentAsString(nameof(StopKey), ""));
            MuteVolumeKey = KeyCombination.FromString(r.ReadElementContentAsString(nameof(MuteVolumeKey), ""));
            IncreaseVolumeKey = KeyCombination.FromString(r.ReadElementContentAsString(nameof(IncreaseVolumeKey), ""));
            DecreaseVolumeKey = KeyCombination.FromString(r.ReadElementContentAsString(nameof(DecreaseVolumeKey), ""));
            r.ReadEndElement();
            //</KeyBindings>
        }

        protected override void WriteXmlSettings(XmlWriter w)
        {
            base.WriteXmlSettings(w);

            //<KeyBindings>
            w.WriteStartElement("KeyBindings");

            w.WriteStartElement(nameof(HandlesKeyEvents));
            w.WriteValue(HandlesKeyEvents);
            w.WriteEndElement();

            w.WriteElementString(nameof(PlayPausedKey), PlayPausedKey.ToString());
            w.WriteElementString(nameof(PauseKey), PauseKey.ToString());
            w.WriteElementString(nameof(StopKey), StopKey.ToString());
            w.WriteElementString(nameof(MuteVolumeKey), MuteVolumeKey.ToString());
            w.WriteElementString(nameof(IncreaseVolumeKey), IncreaseVolumeKey.ToString());
            w.WriteElementString(nameof(DecreaseVolumeKey), DecreaseVolumeKey.ToString());
            w.WriteEndElement();
            //</KeyBindings>

            //w.WriteElementString(nameof(HandlesKeyEvents), HandlesKeyEvents.ToString());
        }

        //public override Dictionary<string, string> Read()
        //{
        //    Dictionary<string, string> settings = base.Read();

        //    settings.Add(nameof(HandlesKeyEvents), HandlesKeyEvents.ToString());
        //    settings.Add(nameof(PlayPausedKey), PlayPausedKey.ToString());
        //    settings.Add(nameof(PauseKey), PauseKey.ToString());
        //    settings.Add(nameof(StopKey), StopKey.ToString());
        //    settings.Add(nameof(MuteVolumeKey), MuteVolumeKey.ToString());
        //    settings.Add(nameof(IncreaseVolumeKey), IncreaseVolumeKey.ToString());
        //    settings.Add(nameof(DecreaseVolumeKey), DecreaseVolumeKey.ToString());

        //    return settings;
        //}

        //public override void Write(Dictionary<string, string> settings)
        //{
        //    base.Write(settings);

        //    HandlesKeyEvents = bool.Parse(settings[nameof(HandlesKeyEvents)]);
        //    PlayPausedKey = KeyCombination.FromString(settings[nameof(PlayPausedKey)]);
        //    PauseKey = KeyCombination.FromString(settings[nameof(PauseKey)]);
        //    StopKey = KeyCombination.FromString(settings[nameof(StopKey)]);
        //    MuteVolumeKey = KeyCombination.FromString(settings[nameof(MuteVolumeKey)]);
        //    IncreaseVolumeKey = KeyCombination.FromString(settings[nameof(IncreaseVolumeKey)]);
        //    DecreaseVolumeKey = KeyCombination.FromString(settings[nameof(DecreaseVolumeKey)]);
        //}

        public override void Dispose()
        {
            _globalKeyEvents.KeyDown -= _globalKeyEvents_KeyDown;
            _globalKeyEvents.Dispose();
        }

        #endregion

        #region Private methods
        private void _globalKeyEvents_KeyDown(object sender, Gma.System.MouseKeyHook.KeyEventArgsExt e)
        {
            if (!HandlesKeyEvents || IsBusy) return;

            KeyCombination combination =
                KeyCombination.FromString(KeyCombination.StringFromKeys(e.KeyCode, e.Modifiers));

            if (PlayStateKeysPressed(combination)) return;
            if (VolumeKeysPressed(combination)) return;

            BoundToSoundKeyPressed(combination);
        }

        private bool PlayStateKeysPressed(KeyCombination combination)
        {
            if (PlayPausedKey != null && PlayPausedKey.Equals(combination))
            {
                PlayPaused();
                return true;
            }
            else if (PauseKey != null && PauseKey.Equals(combination))
            {
                PauseAll();
                return true;
            }
            else if (StopKey != null && StopKey.Equals(combination))
            {
                StopAll();
                return true;
            }

            return false;
        }

        private bool VolumeKeysPressed(KeyCombination combination)
        {
            if (IncreaseVolumeKey != null && IncreaseVolumeKey.Equals(combination))
            {
                IncreaseVolume();
                return true;
            }
            else if (DecreaseVolumeKey != null && DecreaseVolumeKey.Equals(combination))
            {
                DecreaseVolume();
                return true;
            }
            else if (MuteVolumeKey != null && MuteVolumeKey.Equals(combination))
            {
                IsMuted = !IsMuted;
                return true;
            }

            return false;
        }

        private void BoundToSoundKeyPressed(KeyCombination combination)
        {
            foreach (var sound in Sounds.Result)
            {
                if (sound.IsAssociatedWith(combination))
                {
                    sound.Play();
                    return;
                }
            }
        }
        #endregion

    }
}
