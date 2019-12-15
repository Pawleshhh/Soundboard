using System;
using System.Collections.Generic;

namespace ManiacSoundboard.Model
{
    public abstract class Soundboard : IDisposable
    {

        #region Constructors

        protected Soundboard()
        {

            allPlayers = new List<IPlayer>(MaxSize);
            _playingPlayers = new PlayingPlayers(MaxSize);
            _pausedPlayers = new PausedPlayers(MaxSize);
        }

        #endregion

        #region Private fields

        protected SoundboardCacheCollection _playingPlayers;

        protected SoundboardCacheCollection _pausedPlayers;

        protected List<IPlayer> allPlayers;

        #endregion

        #region Properties

        public abstract IAudioDeviceService AudioDevices { get; protected set; }

        public static int MaxSize { get; } = 100;

        public IReadOnlyList<IPlayer> AllPlayers => allPlayers;

        public abstract bool IsFirstDeviceEnabled { get; set; }

        public abstract bool IsSecondDeviceEnabled { get; set; }

        public abstract IAudioDevice FirstDevice { get; set; }

        public abstract IAudioDevice SecondDevice { get; set; }

        public abstract float Volume { get; set; }

        private float volumeStep;

        public float VolumeStep
        {
            get => volumeStep;
            set
            {
                if (value > 1f) volumeStep = 1f;
                else if (value < 0f) volumeStep = 0f;
                else volumeStep = value;
            }
        }

        public abstract bool IsMuted { get; set; }

        public bool IsPlaying => _playingPlayers.Count > 0;

        #endregion

        #region Methods

        public void AddSound(string path)
        {
            IPlayer player = GetPlayer(path);
            _SetupSound(player);
            allPlayers.Add(player);
        }

        public void AddRangeOfSounds(IEnumerable<string> paths)
        {
            foreach (var path in paths)
                AddSound(path);
        }

        public void AddSound(IPlayer player)
        {
            _SetupSound(player);
            allPlayers.Add(player);
        }

        public void AddRangeOfSounds(IEnumerable<IPlayer> players)
        {
            foreach (var sound in players)
                AddSound(sound);
        }

        public void RemoveSound(IPlayer player)
        {
            _DisposeSound(player);
            allPlayers.Remove(player);
        }

        public void RemoveRangeOfSounds(IEnumerable<IPlayer> players)
        {
            foreach (var sound in players)
            {
                _DisposeSound(sound);
                RemoveSound(sound);
            }
        }

        public void RemoveAllSounds()
        {
            StopAll();
            allPlayers.ForEach(n => _DisposeSound(n));
            allPlayers.Clear();
        }

        public void PlayPaused()
        {
            while (_pausedPlayers.Count > 0)
            {
                _pausedPlayers[0].Play();
            }
            //foreach (AudioPlayer player in _pausedPlayers)
            //    player.Play();
        }

        public void PauseAll()
        {
            while (_playingPlayers.Count > 0)
            {
                _playingPlayers[0].Pause();
            }
            //foreach (AudioPlayer player in _playingPlayers)
            //    player.Pause();
        }

        public void StopAll()
        {
            while (_playingPlayers.Count > 0)
            {
                _playingPlayers[0].Stop();
            }

            while (_pausedPlayers.Count > 0)
            {
                _pausedPlayers[0].Stop();
            }
        }

        public void IncreaseVolume()
        {
            Volume += VolumeStep;
        }

        public void DecreaseVolume()
        {
            Volume -= VolumeStep;
        }

        public abstract IPlayer GetPlayer(string path);

        public abstract IAudioDeviceService GetDeviceService();

        public abstract void Dispose();

        #endregion

        #region Private methods

        private void _SetupSound(IPlayer player)
        {
            player.InitPlayer();
            player.FirstDevice = FirstDevice;
            player.SecondDevice = SecondDevice;
            player.IsFirstDeviceEnabled = IsFirstDeviceEnabled;
            player.IsSecondDeviceEnabled = IsSecondDeviceEnabled;
            //sound.BoundData.Volume = Volume;

            player.AudioPlayed += Player_AudioPlayed;
            player.AudioPaused += Player_AudioPaused;
            //player.AudioStopped += Player_AudioStopped;
        }

        private void _DisposeSound(IPlayer player)
        {
            player.AudioPlayed -= Player_AudioPlayed;
            player.AudioPaused -= Player_AudioPaused;
            player.Dispose();
        }

        private void Player_AudioPlayed(object sender, EventArgs e)
        {
            _playingPlayers.Add((IPlayer)sender);
        }

        private void Player_AudioPaused(object sender, EventArgs e)
        {
            _pausedPlayers.Add((IPlayer)sender);
        }

        #endregion

    }
}
