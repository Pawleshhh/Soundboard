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

        public string AudioPath => Player.AudioPath;

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

        public TimeSpan TotalTime => Player.TotalTime;

        public PlayerState State => Player.State;

        public bool IsPlaying => Player.State == PlayerState.Playing ? true : false;

        public bool IsPaused => Player.State == PlayerState.Paused ? true : false;

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

        public void Play()
        {
            Player.Play();
        }

        public void Pause()
        {
            Player.Pause();
        }

        public void Stop()
        {
            Player.Stop();
        }

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

        //public virtual XmlSchema GetSchema()
        //{
        //    return null;
        //}

        //public virtual void ReadXml(XmlReader reader)
        //{
        //    var soundboard = (SoundboardViewModel)Owner;

        //    reader.ReadStartElement();

        //    string path = reader.ReadElementContentAsString("Path", "");
        //    string tag = reader.ReadElementContentAsString("TagName", "");

        //    reader.ReadEndElement();
        //}

        //public virtual void WriteXml(XmlWriter writer)
        //{
        //    writer.WriteElementString("Path", AudioPath);
        //    writer.WriteAttributeString("TagName", TagName);
        //}

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

        public static IPlayer GetPlayer(SoundViewModel soundVM)
        {
            return soundVM.Player;
        }

        #endregion

    }
}
