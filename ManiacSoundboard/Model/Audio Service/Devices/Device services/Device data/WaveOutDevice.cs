using NAudio.Wave;
using System;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Represents an audio device for output devices.
    /// </summary>
    public class WaveOutDevice : IAudioDevice, IEquatable<WaveOutDevice>
    {

        public WaveOutDevice(WaveOutCapabilities capabilities, int number, string friendlyName)
        {
            Capabilities = capabilities;
            DeviceId = number;
            FriendlyName = friendlyName;
        }

        public WaveOutCapabilities Capabilities { get; }

        public string FriendlyName { get; }

        public object DeviceId { get; }

        public bool Equals(WaveOutDevice other)
        {
            return Capabilities.Equals(other.Capabilities);
        }

        public bool Equals(IAudioDevice other)
        {
            if (!(other is WaveOutDevice)) return false;

            return Equals((WaveOutDevice)other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is WaveOutDevice)) return false;

            return Equals((WaveOutDevice)obj);
        }

        public override int GetHashCode()
        {
            return Capabilities.GetHashCode();
        }

        public override string ToString()
        {
            return FriendlyName;
        }

    }
}
