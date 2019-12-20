using ManiacSoundboard.Model;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ManiacSoundboard.ViewModel
{

    /// <summary>
    /// Base class of sound view model.
    /// </summary>
    public class SoundViewModel : BaseViewModel, IEquatable<SoundViewModel>, IXmlReadableSerialization
    {

        #region Constructors

        protected SoundViewModel() : base()
        {

        }

        public SoundViewModel(IPlayer player, SoundboardViewModel soundboardViewModel) : base(soundboardViewModel)
        {
            _soundboardVM = soundboardViewModel;
            Player = player;
            _SubscribeSoundEvents();
        }

        //public SoundViewModel(string audioPath, SoundboardViewModel soundBoardViewModel) : this(new Sound(audioPath), soundBoardViewModel)
        //{

        //}

        #endregion

        #region Private fields

        private SoundboardViewModel _soundboardVM;

        #endregion

        #region Properties

        private IPlayer Player { get; set; }

        /// <summary>
        /// Gets audio path.
        /// </summary>
        public string AudioPath => Player.AudioPath;

        /// <summary>
        /// Gets or sets current time of current player.
        /// </summary>
        public TimeSpan CurrentTime
        {
            get
            {
                if (_soundboardVM.IsChangingDevice)
                    return TimeSpan.Zero;

                return Player.CurrentTime;
            }
            set
            {
                Player.CurrentTime = value;
                OnPropertyChanged("CurrentTime");
            }
        }

        /// <summary>
        /// Gets total time of audio.
        /// </summary>
        public TimeSpan TotalTime => Player.TotalTime;

        /// <summary>
        /// Gets state of the player.
        /// </summary>
        public PlayerState State => Player.State;

        /// <summary>
        /// Gets whether the player is playing or not.
        /// </summary>
        public bool IsPlaying => Player.State == PlayerState.Playing ? true : false;

        /// <summary>
        /// Gets whether the player is paused or not.
        /// </summary>
        public bool IsPaused => Player.State == PlayerState.Paused ? true : false;

        /// <summary>
        /// Gets or sets the tag name of the player.
        /// </summary>
        public string TagName
        {
            get => Player.TagName;
            set
            {
                Player.TagName = value;
                OnPropertyChanged("TagName");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Plays audio.
        /// </summary>
        public void Play()
        {
            Player.Play();
        }

        /// <summary>
        /// Pauses audio.
        /// </summary>
        public void Pause()
        {
            Player.Pause();
        }

        /// <summary>
        /// Stops audio.
        /// </summary>
        public void Stop()
        {
            Player.Stop();
        }

        /// <summary>
        /// Helper method to specify if this view model is associated with some data.
        /// </summary>
        public virtual bool IsAssociatedWith(object data)
        {
            return false;
        }

        public bool Equals(SoundViewModel other)
        {
            return Player.Equals(other.Player);
        }

        public override bool Equals(object obj)
        {
            if (obj is SoundViewModel soundViewModel) return Equals(soundViewModel);

            return false;
        }

        public override int GetHashCode()
        {
            return Player.GetHashCode();
        }

        public override string ToString()
        {
            return Player.ToString();
        }

        public override void Dispose()
        {
            Player.AudioPlayed -= Data_AudioPlayed;
            Player.AudioPaused -= Data_AudioPaused;
            Player.AudioStopped -= Data_AudioStopped;
            Player.AudioAutoStopped -= Data_AudioAutoStopped;

            //_sound = null;
            _soundboardVM = null;
        }

        public virtual Dictionary<string, string> LoadData()
        {
            var settings = new Dictionary<string, string>();

            settings.Add("AudioPath", AudioPath);
            settings.Add("TagName", TagName);

            return settings;
        }

        #endregion

        #region Private methods

        private void _SubscribeSoundEvents()
        {
            Player.AudioPlayed += Data_AudioPlayed;
            Player.AudioPaused += Data_AudioPaused;
            Player.AudioStopped += Data_AudioStopped;
            Player.AudioAutoStopped += Data_AudioAutoStopped;
        }

        private void Data_AudioAutoStopped(object sender, EventArgs e)
        {
            OnPropertyChanged("IsPlaying", "IsPaused", "State", "CurrentTime");
        }

        private void Data_AudioStopped(object sender, EventArgs e)
        {
            OnPropertyChanged("IsPlaying", "IsPaused", "State", "CurrentTime");
        }

        private void Data_AudioPaused(object sender, EventArgs e)
        {
            OnPropertyChanged("IsPlaying", "IsPaused", "State", "CurrentTime");
        }

        private void Data_AudioPlayed(object sender, EventArgs e)
        {
            OnPropertyChanged("IsPlaying", "IsPaused", "State", "CurrentTime");
        }

        #endregion

        #region Commands

        private ICommand playCommand;

        /// <summary>
        /// Gets command that plays audio or pauses it if it's already playing.
        /// </summary>
        public ICommand PlayCommand
        {
            get
            {
                if (playCommand == null)
                    playCommand = new RelayCommand(o =>
                    {
                        if (Player.State == PlayerState.Playing)
                            Pause();
                        else
                            Play();
                    });

                return playCommand;
            }
        }

        private ICommand playStopCommand;

        /// <summary>
        /// Gets command that plays audio or stops it if it's already playing.
        /// </summary>
        public ICommand PlayStopCommand
        {
            get
            {
                if (playStopCommand == null)
                    playStopCommand = new RelayCommand(o =>
                    {
                        if (Player.State == PlayerState.Playing)
                            Stop();
                        else
                            Play();
                    });

                return playStopCommand;
            }
        }

        private ICommand pauseCommand;

        /// <summary>
        /// Gets command that pauses audio or plays it if it's already paused.
        /// </summary>
        public ICommand PauseCommand
        {
            get
            {
                if (pauseCommand == null)
                    pauseCommand = new RelayCommand(o =>
                    {
                        if (Player.State == PlayerState.Paused)
                            Play();
                        else
                            Pause();
                    });

                return pauseCommand;
            }
        }

        private ICommand stopCommand;

        /// <summary>
        /// Gets command that stops audio.
        /// </summary>
        public ICommand StopCommand
        {
            get
            {
                if (stopCommand == null)
                    stopCommand = new RelayCommand(o =>
                    {
                        Stop();
                    });

                return stopCommand;
            }
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Gets player from given <see cref="SoundViewModel"/>.
        /// </summary>
        public static IPlayer GetPlayer(SoundViewModel soundVM)
        {
            return soundVM.Player;
        }

        #endregion

    }
}
