using System;
using System.Collections.Generic;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Abstract class that defines main model of soundboard.
    /// </summary>
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

        /// <summary>
        /// Gets <see cref="IAudioDeviceService"/> that is associated with soundboard's api.
        /// </summary>
        public abstract IAudioDeviceService AudioDevices { get; protected set; }

        /// <summary>
        /// Gets max amount of players that can be stored in the soundboard.
        /// </summary>
        public static int MaxSize { get; } = 100;

        /// <summary>
        /// Gets collection of all players.
        /// </summary>
        public IReadOnlyList<IPlayer> AllPlayers => allPlayers;

        /// <summary>
        /// Gets or sets whether the first device is enabled or not.
        /// </summary>
        public abstract bool IsFirstDeviceEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the second device is enabled or not.
        /// </summary>
        public abstract bool IsSecondDeviceEnabled { get; set; }

        /// <summary>
        /// Gets or sets the first device.
        /// </summary>
        public abstract IAudioDevice FirstDevice { get; set; }

        /// <summary>
        /// Gets or sets the second device.
        /// </summary>
        public abstract IAudioDevice SecondDevice { get; set; }

        /// <summary>
        /// Gets or sets volume of soundboard.
        /// </summary>
        public abstract float Volume { get; set; }

        /// <summary>
        /// Gets or sets volume step of de/increasing soundboard's volume.
        /// </summary>
        public abstract float VolumeStep { get; set; }

        /// <summary>
        /// Gets or sets whether the soundboard is muted or not.
        /// </summary>
        public abstract bool IsMuted { get; set; }

        /// <summary>
        /// Gets whether any player is playing or not.
        /// </summary>
        public bool IsPlaying => _playingPlayers.Count > 0;

        #endregion

        #region Methods

        /// <summary>
        /// Adds new player by path to audio file.
        /// </summary>
        /// <param name="path">Path to audio file.</param>
        public void AddSound(string path)
        {
            IPlayer player = GetPlayer(path);
            _SetupSound(player);
            allPlayers.Add(player);
        }

        /// <summary>
        /// Adds multiple players by given paths to audio files.
        /// </summary>
        /// <param name="paths">Paths to audio files.</param>
        public void AddRangeOfSounds(IEnumerable<string> paths)
        {
            foreach (var path in paths)
                AddSound(path);
        }

        /// <summary>
        /// Adds player.
        /// </summary>
        /// <param name="player">Player to be added.</param>
        public void AddSound(IPlayer player)
        {
            _SetupSound(player);
            allPlayers.Add(player);
        }

        /// <summary>
        /// Adds multiple players.
        /// </summary>
        /// <param name="players">Collection of players to be added.</param>
        public void AddRangeOfSounds(IEnumerable<IPlayer> players)
        {
            foreach (var sound in players)
                AddSound(sound);
        }

        /// <summary>
        /// Removes given player.
        /// </summary>
        /// <param name="player">Player to be removed.</param>
        public void RemoveSound(IPlayer player)
        {
            _DisposeSound(player);
            allPlayers.Remove(player);
        }

        /// <summary>
        /// Removes range of players.
        /// </summary>
        /// <param name="players">Players to be removed.</param>
        public void RemoveRangeOfSounds(IEnumerable<IPlayer> players)
        {
            foreach (var sound in players)
            {
                _DisposeSound(sound);
                RemoveSound(sound);
            }
        }

        /// <summary>
        /// Removes all players.
        /// </summary>
        public void RemoveAllSounds()
        {
            StopAll();
            allPlayers.ForEach(n => _DisposeSound(n));
            allPlayers.Clear();
        }

        /// <summary>
        /// Plays all paused players.
        /// </summary>
        public void PlayPaused()
        {
            while (_pausedPlayers.Count > 0)
            {
                _pausedPlayers[0].Play();
            }
        }

        /// <summary>
        /// Pauses all playing players.
        /// </summary>
        public void PauseAll()
        {
            while (_playingPlayers.Count > 0)
            {
                _playingPlayers[0].Pause();
            }
        }

        /// <summary>
        /// Stops all playing and paused players.
        /// </summary>
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

        /// <summary>
        /// Increases <see cref="Volume"/> by <see cref="VolumeStep"/>/
        /// </summary>
        public void IncreaseVolume()
        {
            Volume += VolumeStep;
        }

        /// <summary>
        /// Decreases <see cref="Volume"/> by <see cref="VolumeStep"/>/
        /// </summary>
        public void DecreaseVolume()
        {
            Volume -= VolumeStep;
        }

        /// <summary>
        /// Gets player that is associated with the implementation of this soundboard.
        /// </summary>
        /// <param name="path">Path of the audio file.</param>
        public abstract IPlayer GetPlayer(string path);

        /// <summary>
        /// Gets audio service thath is associated with the implementation of this soundboard.
        /// </summary>
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
