using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacSoundboard.Model
{
    public interface IPlayer : IDisposable
    {

        string AudioPath { get; }

        string TagName { get; set; }

        float Volume { get; set; }

        bool IsFirstDeviceEnabled { get; set; }

        bool IsSecondDeviceEnabled { get; set; }

        IAudioDevice FirstDevice { get; set; }

        IAudioDevice SecondDevice { get; set; }

        PlayerState State { get; }

        TimeSpan CurrentTime { get; set; }

        TimeSpan TotalTime { get; }

        event EventHandler AudioPlayed;

        event EventHandler AudioPaused;

        event EventHandler AudioStopped;

        event EventHandler AudioAutoStopped;

        void InitPlayer();

        void Play();

        void Pause();

        void Stop();

    }

    public enum PlayerState
    {
        Stopped = 0,
        Playing = 1,
        Paused = 2
    }
}
