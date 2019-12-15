using NAudio.Wave;
using System;
using System.IO;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Provides functions to play audio on two different audio devices.
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

        public string TagName { get; set; }

        /// <summary>
        /// Gets or sets volume of being played audio (from 0 to 1.0f).
        /// </summary>
        public float Volume
        {
            get => _volume;
            set
            {
                if (value < 0) _volume = 0f;
                else if (value > 1.0f) _volume = 1f;
                else _volume = value;

                if (_firstAudioFileReader != null) _firstAudioFileReader.Volume = _volume;
                if (_secondAudioFileReader != null) _secondAudioFileReader.Volume = _volume;
            }
        }

        private bool isFirstWaveOutDeviceEnabled = true;

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
                Stop();

                _firstDevice = value;

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
                Stop();

                _secondDevice = value;

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
                if (IsFirstDeviceEnabled && _firstPlayer != null) return (PlayerState)_firstPlayer.PlaybackState;
                if (IsSecondDeviceEnabled && _secondPlayer != null) return (PlayerState)_secondPlayer.PlaybackState;

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
                if (IsFirstDeviceEnabled && _firstWaveStream != null)
                    return _firstWaveStream.CurrentTime;
                if (IsSecondDeviceEnabled && _secondWaveStream != null)
                    return _secondWaveStream.CurrentTime;

                return TimeSpan.Zero;
            }
            set
            {
                //if (_firstDevice != null)
                //    _firstWaveChannel.CurrentTime = value;
                //if (_secondDevice != null)
                //    _secondWaveChannel.CurrentTime = value;
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
                if (IsFirstDeviceEnabled)
                    return _firstWaveStream.TotalTime;
                else if (IsSecondDeviceEnabled)
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

        public void InitPlayer()
        {
            _InitFirstPlayer();
            _InitSecondPlayer();
        }

        public void Play()
        {
            if (State == PlayerState.Playing) Stop();
            if (CurrentTime >= TotalTime)
            {
                Stop();
                return;
            }

            if (IsFirstDeviceEnabled)
                _firstPlayer.Play();
            if (IsSecondDeviceEnabled)
                _secondPlayer.Play();

            OnAudioPlayed();
        }

        public void Pause()
        {
            if (State != PlayerState.Playing) return;

            if (IsFirstDeviceEnabled)
                _firstPlayer.Pause();
            if (IsSecondDeviceEnabled)
                _secondPlayer.Pause();

            OnAudioPaused();
        }

        public void Stop()
        {
            if (IsFirstDeviceEnabled)
                _firstPlayer.Stop();
            if (IsSecondDeviceEnabled)
                _secondPlayer.Stop();

            CurrentTime = TimeSpan.FromSeconds(0);

            OnAudioStopped();
        }

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
            if (obj is WaveEventPlayer audioPlayer) return AudioPath.Equals(audioPlayer.AudioPath);

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
                waveOut.DeviceNumber = _firstDevice.DeviceId;
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
                waveOut.DeviceNumber = _secondDevice.DeviceId;
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
