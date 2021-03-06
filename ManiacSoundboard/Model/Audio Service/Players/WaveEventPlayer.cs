﻿using NAudio.Wave;
using System;
using System.IO;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Player that use <see cref="WaveOutEvent"/> api.
    /// </summary>
    public class WaveEventPlayer : IPlayer
    {

        #region Constructors

        /// <summary>
        /// Initializes an instance of the <see cref="WaveEventPlayer"/> class.
        /// </summary>
        /// <param name="audioPath">Path of an audio to be played.</param>
        /// <exception cref="FileNotFoundException"/>
        public WaveEventPlayer(string audioPath)
        {
            //if (!File.Exists(audioPath))
            //    throw new FileNotFoundException($"The {audioPath} does not exist.");

            AudioPath = audioPath;

            InitPlayer();
        }

        #endregion

        #region Private fields

        private float _volume = 1f;

        private bool _initFirstTimeFirstDevice = true;

        private bool _initFirstTimeSecondDevice = true;

        private bool _firstPlayerDisposed = false;

        private bool _secondPlayerDisposed = false;

        private IWavePlayer _firstPlayer;

        private IWavePlayer _secondPlayer;

        private WaveStream _firstWaveStream;

        private WaveStream _secondWaveStream;

        private AudioFileReader _firstAudioFileReader;

        private AudioFileReader _secondAudioFileReader;

        private WaveChannel32 _firstWaveChannel;

        private WaveChannel32 _secondWaveChannel;

        private IAudioDevice _firstDevice;

        private IAudioDevice _secondDevice;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the path of the audio.
        /// </summary>
        public string AudioPath { get; private set; }

        /// <summary>
        /// Gets or sets tag name.
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// Gets or sets volume of being played audio (from 0 to 1.0f).
        /// </summary>
        public float Volume
        {
            get => _volume;
            set
            {
                //Value cannot be less than 0 and greater than 1
                if (value < 0) _volume = 0f;
                else if (value > 1.0f) _volume = 1f;
                else _volume = value;

                //If there are initialized file readers then set their volume.
                if (_firstAudioFileReader != null) _firstAudioFileReader.Volume = _volume;
                if (_secondAudioFileReader != null) _secondAudioFileReader.Volume = _volume;
            }
        }

        private bool isFirstWaveOutDeviceEnabled = true;

        /// <summary>
        /// Gets or sets whether the first device is enabled or not (does not change value of the device).
        /// </summary>
        public bool IsFirstDeviceEnabled
        {
            get
            {
                if (FirstDevice == null) return false;

                return isFirstWaveOutDeviceEnabled;
            }
            set => isFirstWaveOutDeviceEnabled = value;
        }

        private bool isSecondWaveOutDeviceEnabled = true;

        /// <summary>
        /// Gets or sets whether the second device is enabled or not (does not change value of the device).
        /// </summary>
        public bool IsSecondDeviceEnabled
        {
            get
            {
                if (SecondDevice == null) return false;

                return isSecondWaveOutDeviceEnabled;
            }
            set => isSecondWaveOutDeviceEnabled = value;
        }

        /// <summary>
        /// Gets or sets an audio device for the first player.
        /// </summary>
        public IAudioDevice FirstDevice
        {
            get => _firstDevice;
            set
            {
                //Stop audio before the first device changes
                Stop();

                _firstDevice = value;

                //If the new device is not null then initialize it
                if (_firstDevice != null)
                {
                    _InitFirstDevice();
                }
            }
        }

        /// <summary>
        /// Gets or sets an audio device for the second player.
        /// </summary>
        public IAudioDevice SecondDevice
        {
            get => _secondDevice;
            set
            {
                //Stop audio before the second device changes
                Stop();

                _secondDevice = value;

                //If the new device is not null then initialize it
                if (_secondDevice != null)
                {
                    _InitSecondDevice();
                }
            }
        }

        /// <summary>
        /// Gets state of the playback.
        /// </summary>
        public PlayerState State
        {
            get
            {
                //If the first device is enabled and the first player is not null then return state of that player.
                if (IsFirstDeviceEnabled && _firstPlayer != null) return (PlayerState)_firstPlayer.PlaybackState;
                //Same with the second device and second player
                if (IsSecondDeviceEnabled && _secondPlayer != null) return (PlayerState)_secondPlayer.PlaybackState;

                //Otherwise return the stopped state.
                return PlayerState.Stopped;
            }
        }

        /// <summary>
        /// Current time of the audio.
        /// </summary>
        public TimeSpan CurrentTime
        {
            get
            {
                //Get current time of the first wave stream or the second one
                if (IsFirstDeviceEnabled && _firstWaveStream != null)
                    return _firstWaveStream.CurrentTime;
                if (IsSecondDeviceEnabled && _secondWaveStream != null)
                    return _secondWaveStream.CurrentTime;

                //Otherwise get TimeSpan.Zero
                return TimeSpan.Zero;
            }
            set
            {
                //if (_firstDevice != null)
                //    _firstWaveChannel.CurrentTime = value;
                //if (_secondDevice != null)
                //    _secondWaveChannel.CurrentTime = value;
                
                //Set the position by the given TimeSpan
                SetPosition(value);
            }
        }

        /// <summary>
        /// Gets the total time of the audio.
        /// </summary>
        public TimeSpan TotalTime
        {
            get
            {
                //Get the total time of one of the wave streams.
                if (_firstWaveStream != null)
                    return _firstWaveStream.TotalTime;
                else if (_secondWaveStream != null)
                    return _secondWaveStream.TotalTime;
                
                return TimeSpan.Zero;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raises when audio starts to play.
        /// </summary>
        public event EventHandler AudioPlayed;

        /// <summary>
        /// Raises when audio pauses.
        /// </summary>
        public event EventHandler AudioPaused;

        /// <summary>
        /// Raises when audio stops.
        /// </summary>
        public event EventHandler AudioStopped;

        /// <summary>
        /// Raises when audio stops automatically
        /// </summary>
        public event EventHandler AudioAutoStopped;

        private void OnAudioPlayed()
        {
            AudioPlayed?.Invoke(this, EventArgs.Empty);
        }

        private void OnAudioPaused()
        {
            AudioPaused?.Invoke(this, EventArgs.Empty);
        }

        private void OnAudioStopped()
        {
            AudioStopped?.Invoke(this, EventArgs.Empty);
        }

        private void OnAudioAutoStopped()
        {
            AudioAutoStopped?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inits this player.
        /// </summary>
        public void InitPlayer()
        {
            _InitFirstPlayer();
            _InitSecondPlayer();
        }

        /// <summary>
        /// Plays the audio.
        /// </summary>
        public void Play()
        {
            //Player is already playing so stop it and play it again.
            if (State == PlayerState.Playing) Stop();
            
            //Just in case
            if (CurrentTime >= TotalTime)
            {
                Stop();
                return;
            }

            //If the first device is enabled then play the audio on it.
            if (IsFirstDeviceEnabled)
                _firstPlayer.Play();
            //Same with the second one
            if (IsSecondDeviceEnabled)
                _secondPlayer.Play();

            //Invoke the event
            OnAudioPlayed();
        }

        /// <summary>
        /// Pauses the audio.
        /// </summary>
        public void Pause()
        {
            //Audio is not playing then audio cannot be paused.
            if (State != PlayerState.Playing) return;

            //If the first device is enabled then pause audio on it
            if (IsFirstDeviceEnabled)
                _firstPlayer.Pause();
            //Same with the second one
            if (IsSecondDeviceEnabled)
                _secondPlayer.Pause();

            //Invoke the event
            OnAudioPaused();
        }

        /// <summary>
        /// Stops the audio.
        /// </summary>
        public void Stop()
        {
            //If the first device is enabled then stop the audio it plays.
            if (IsFirstDeviceEnabled)
                _firstPlayer.Stop();
            //Same with the second one
            if (IsSecondDeviceEnabled)
                _secondPlayer.Stop();

            //Set current time to 0
            CurrentTime = TimeSpan.FromSeconds(0);

            //Invoke the event
            OnAudioStopped();
        }

        /// <summary>
        /// Sets the position (time) of the stored audio.
        /// </summary>
        /// <param name="seconds">Seconds in <see cref="double"/> type.</param>
        public void SetPosition(double seconds)
        {
            WaveStream strm;
            if (IsFirstDeviceEnabled)
                strm = _firstWaveStream;
            else if (IsSecondDeviceEnabled)
                strm = _secondWaveStream;
            else
                return;

            _SetPosition(strm, seconds);
        }

        /// <summary>
        /// Sets the position (time) of the stored audio.
        /// </summary>
        /// <param name="time">Time in <see cref="TimeSpan"/> type.</param>
        public void SetPosition(TimeSpan time)
        {
            WaveStream strm;
            if (IsFirstDeviceEnabled)
                strm = _firstWaveStream;
            else if (IsSecondDeviceEnabled)
                strm = _secondWaveStream;
            else
                return;

            _SetPosition(strm, time);
        }

        public void Dispose()
        {
            _DisposeFirstPlayer();
            _DisposeSecondPlayer();
        }

        public override bool Equals(object obj)
        {
            if (obj is WaveEventPlayer audioPlayer) 
                return AudioPath.Equals(audioPlayer.AudioPath);

            return false;
        }

        public override int GetHashCode()
        {
            return AudioPath.GetHashCode() * 11;
        }

        public override string ToString()
        {
            return AudioPath;
        }

        #endregion

        #region Private methods

        private void Players_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (_firstPlayerDisposed && _secondPlayerDisposed) return;
            if (CurrentTime >= TotalTime)
            {
                Stop();
            }
        }

        private void _SetPosition(WaveStream strm, long position)
        {
            // distance from block boundary (may be 0)
            long adj = position % strm.WaveFormat.BlockAlign;
            // adjust position to boundary and clamp to valid range
            long newPos = Math.Max(0, Math.Min(strm.Length, position - adj));
            // set playback position
            if(_firstDevice != null)
                _firstWaveStream.Position = newPos;
            if(_secondDevice != null)
                _secondWaveStream.Position = newPos;
        }

        private void _SetPosition(WaveStream strm, double seconds)
        {
            _SetPosition(strm, (long)(seconds * strm.WaveFormat.AverageBytesPerSecond));
        }

        private void _SetPosition(WaveStream strm, TimeSpan time)
        {
            _SetPosition(strm, time.TotalSeconds);
        }

        private void _InitFirstDevice()
        {
            if(_firstDevice != null)
            {
                if (_firstPlayer != null)
                {
                    _firstPlayer.PlaybackStopped -= Players_PlaybackStopped;
                    _firstPlayer.Dispose();
                }
                WaveOutEvent waveOut = new WaveOutEvent();
                waveOut.DeviceNumber = (int)_firstDevice.DeviceId;
                waveOut.PlaybackStopped += Players_PlaybackStopped;
                _firstPlayer = waveOut;
                _firstPlayer.Init(_firstWaveChannel);
            }
        }

        private void _InitSecondDevice()
        {
            if(_secondDevice != null)
            {
                if (_secondPlayer != null)
                {
                    _secondPlayer.PlaybackStopped -= Players_PlaybackStopped;
                    _secondPlayer.Dispose();
                }

                WaveOutEvent waveOut = new WaveOutEvent();
                waveOut.DeviceNumber = (int)_secondDevice.DeviceId;
                waveOut.PlaybackStopped += Players_PlaybackStopped;
                _secondPlayer = waveOut;
                _secondPlayer.Init(_secondWaveChannel);
            }
        }

        private void _InitFirstPlayer()
        {
            if (!_firstPlayerDisposed && !_initFirstTimeFirstDevice) _DisposeFirstPlayer();
            else 
                _initFirstTimeFirstDevice = false;

            //if (_firstDevice == null) return;

            AudioFileReader audioFileReader = new AudioFileReader(AudioPath);
            audioFileReader.Volume = Volume;
            WaveStream stream = audioFileReader;
            WaveChannel32 channel = new WaveChannel32(stream);
            channel.PadWithZeroes = false;

            _firstWaveChannel = channel;
            _firstAudioFileReader = audioFileReader;
            _firstWaveStream = stream;

            _InitFirstDevice();

            _firstPlayerDisposed = false;
            //CurrentTime = TimeSpan.FromSeconds(0);
        }

        private void _InitSecondPlayer()
        {
            if (!_secondPlayerDisposed && !_initFirstTimeSecondDevice) _DisposeSecondPlayer();
            else
                _initFirstTimeSecondDevice = false;

            //if (_secondDevice == null) return;

            AudioFileReader audioFileReader = new AudioFileReader(AudioPath);
            audioFileReader.Volume = Volume;
            WaveStream stream = audioFileReader;
            WaveChannel32 channel = new WaveChannel32(stream);
            channel.PadWithZeroes = false;

            _secondWaveChannel = channel;
            _secondAudioFileReader = audioFileReader;
            _secondWaveStream = stream;

            _InitSecondDevice();
            _secondPlayerDisposed = false;
            //CurrentTime = TimeSpan.FromSeconds(0);
        }

        private void _DisposeFirstPlayer()
        {
            if (_firstPlayerDisposed) return;

            if(_firstPlayer != null)
                _firstPlayer.PlaybackStopped -= Players_PlaybackStopped;

            _firstWaveStream?.Dispose();
            _firstAudioFileReader?.Dispose();
            _firstWaveChannel?.Dispose();
            _firstPlayer?.Dispose();

            _firstWaveStream = null;
            _firstAudioFileReader = null;
            _firstWaveChannel = null;
            _firstPlayer = null;

            _firstPlayerDisposed = true;
        }

        private void _DisposeSecondPlayer()
        {
            if (_secondPlayerDisposed) return;

            if(_secondPlayer != null)
                _secondPlayer.PlaybackStopped -= Players_PlaybackStopped;

            _secondWaveStream?.Dispose();
            _secondAudioFileReader?.Dispose();
            _secondWaveChannel?.Dispose();
            _secondPlayer?.Dispose();

            _secondWaveStream = null;
            _secondAudioFileReader = null;
            _secondWaveChannel = null;
            _secondPlayer = null;

            _secondPlayerDisposed = true;
        }

        #endregion

    }
}
