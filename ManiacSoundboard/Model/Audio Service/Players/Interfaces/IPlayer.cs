using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Defines interface for players.
    /// </summary>
    public interface IPlayer : IDisposable
    {

        /// <summary>
        /// Gets path to audio this player plays.
        /// </summary>
        string AudioPath { get; }

        /// <summary>
        /// Gets or sets tag name.
        /// </summary>
        string TagName { get; set; }

        /// <summary>
        /// Gets or sets volume of this player.
        /// </summary>
        float Volume { get; set; }

        /// <summary>
        /// Gets or sets whether the first device is enabled or not (does not change value of the device).
        /// </summary>
        bool IsFirstDeviceEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the second device is enabled or not (does not change value of the device).
        /// </summary>
        bool IsSecondDeviceEnabled { get; set; }

        /// <summary>
        /// Gets or sets first device.
        /// </summary>
        IAudioDevice FirstDevice { get; set; }

        /// <summary>
        /// Gets or sets second device.
        /// </summary>
        IAudioDevice SecondDevice { get; set; }

        /// <summary>
        /// Gets state of the player.
        /// </summary>
        PlayerState State { get; }

        /// <summary>
        /// Gets or sets current time of audio that this player plays.
        /// </summary>
        TimeSpan CurrentTime { get; set; }

        /// <summary>
        /// Gets total time of the audio this player plays.
        /// </summary>
        TimeSpan TotalTime { get; }

        /// <summary>
        /// Rises when audio has been played by this player.
        /// </summary>
        event EventHandler AudioPlayed;

        /// <summary>
        /// Rises when audio has been paused by this player.
        /// </summary>
        event EventHandler AudioPaused;

        /// <summary>
        /// Rises when audio has been stopped by this player.
        /// </summary>
        event EventHandler AudioStopped;

        /// <summary>
        /// Rises when audio has been automatically stopped by this player.
        /// </summary>
        event EventHandler AudioAutoStopped;

        /// <summary>
        /// Inits this player.
        /// </summary>
        void InitPlayer();

        /// <summary>
        /// Plays the audio.
        /// </summary>
        void Play();

        /// <summary>
        /// Pauses the audio.
        /// </summary>
        void Pause();

        /// <summary>
        /// Stops the audio.
        /// </summary>
        void Stop();

    }

    /// <summary>
    /// Enum of possible player states.
    /// </summary>
    public enum PlayerState
    {
        Stopped = 0,
        Playing = 1,
        Paused = 2
    }
}
