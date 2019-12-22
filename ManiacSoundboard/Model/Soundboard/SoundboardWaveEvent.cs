using ManiacSoundboard.Properties;
using NAudio.Wave;
using System;
using System.Linq;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Implementation of <see cref="Soundboard"/> that uses <see cref="WaveOutEvent"/> from NAudio library to play sounds.
    /// </summary>
    public class SoundboardWaveEvent : Soundboard
    {

        #region Constructors

        public SoundboardWaveEvent()
        {
            //_InitializeDevices();
            AudioDevices = GetDeviceService();
            _blankWav = new WaveFileReader(Resources._1sec);
            
            _InitializeProperties();
        }

        ~SoundboardWaveEvent()
        {
            Dispose();
        }

        #endregion

        #region Private fields

        private bool _disposed = false;

        private WaveFileReader _blankWav;

        private WaveOutEvent _firstDevice;

        private WaveOutEvent _secondDevice;

        private object _firstDeviceLocker = new object();

        private object _secondDeviceLocker = new object();

        #endregion

        #region Properites

        public override IAudioDeviceService AudioDevices { get; protected set; }

        private bool isFirstDeviceEnabled = true;

        public override bool IsFirstDeviceEnabled
        {
            get
            {
                if (firstDevice == null) return false;

                return isFirstDeviceEnabled;
            }
            set
            {
                StopAll();
                isFirstDeviceEnabled = value;

                allPlayers.ForEach(n =>
                {
                    n.IsFirstDeviceEnabled = value;
                });
            }
        }

        private bool isSecondDeviceEnabled = true;

        public override bool IsSecondDeviceEnabled
        {
            get
            {
                if (secondDevice == null) return false;

                return isSecondDeviceEnabled;
            }
            set
            {
                StopAll();
                isSecondDeviceEnabled = value;

                allPlayers.ForEach(n =>
                {
                    n.IsSecondDeviceEnabled = value;
                });
            }
        }

        private IAudioDevice firstDevice;

        public override IAudioDevice FirstDevice
        {
            get => firstDevice;
            set
            {
                firstDevice = value;

                _InitializeFirstDevice();
            }
        }

        private IAudioDevice secondDevice;

        public override IAudioDevice SecondDevice
        {
            get => secondDevice;
            set
            {
                secondDevice = value;

                _InitializeSecondDevice();
            }
        }

        private float volume;

        public override float Volume
        {
            get => volume;
            set
            {
                if (value > 1f) volume = 1f;
                else if (value < 0f) volume = 0f;
                else volume = value;

                if(!IsMuted)
                {
                    if (_firstDevice != null)
                        _firstDevice.Volume = volume;
                    if (_secondDevice != null)
                        _secondDevice.Volume = volume;
                }
            }
        }

        private float volumeStep;

        public override float VolumeStep
        {
            get => volumeStep;
            set
            {
                if (value > 1f) volumeStep = 1f;
                else if (value < 0f) volumeStep = 0f;
                else volumeStep = value;
            }
        }

        private bool isMuted;

        public override bool IsMuted
        {
            get => isMuted;
            set
            {
                isMuted = value;
                if (value)
                {
                    if (_firstDevice != null)
                        _firstDevice.Volume = 0;
                    if (_secondDevice != null)
                        _secondDevice.Volume = 0;
                }
                else
                {
                    Volume = volume;
                }
            }
        }

        #endregion

        #region Methods

        public override IPlayer GetPlayer(string path)
        {
            return new WaveEventPlayer(path);
        }

        public override IAudioDeviceService GetDeviceService()
        {
            return new WaveDevices();
        }

        public override void Dispose()
        {
            if (_disposed) return;

            allPlayers.ForEach(sound => _DisposeSound(sound));
            _playingPlayers.Dispose();
            _pausedPlayers.Dispose();

            _DisposeMainAudioDevices();

            _disposed = true;

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private methods

        private void _InitializeFirstDevice()
        {
            lock(_firstDeviceLocker)
            {
                allPlayers.ForEach(sound =>
                {
                    sound.FirstDevice = FirstDevice;
                });

                if (FirstDevice != null)
                {
                    _firstDevice?.Dispose();
                    _firstDevice = new WaveOutEvent
                    {
                        DeviceNumber = (int)FirstDevice.DeviceId
                    };

                    _firstDevice.Init(_blankWav);
                    _firstDevice.Volume = Volume;
                }
            }
        }

        private void _InitializeSecondDevice()
        {
            lock(_secondDeviceLocker)
            {
                allPlayers.ForEach(sound =>
                {
                    sound.SecondDevice = SecondDevice;
                });

                if (SecondDevice != null)
                {
                    _secondDevice?.Dispose();
                    _secondDevice = new WaveOutEvent
                    {
                        DeviceNumber = (int)SecondDevice.DeviceId
                    };

                    _secondDevice.Init(_blankWav);
                    _secondDevice.Volume = Volume;
                }
            }
        }

        private void _DisposeMainAudioDevices()
        {
            _firstDevice?.Dispose();
            _secondDevice?.Dispose();

            _blankWav?.Dispose();
        }

        private void _InitializeDevices()
        {
            AudioDevices.ReloadOutDevices();

            if (AudioDevices.OutDevices.Count > 0)
                FirstDevice = AudioDevices.OutDevices.First();
            if (AudioDevices.OutDevices.Count > 1)
                SecondDevice = AudioDevices.OutDevices.ElementAt(1);
        }

        private void _InitializeProperties()
        {
            VolumeStep = 0.05f;
            volume = 0.3f;
        }

        #endregion

    }
}
