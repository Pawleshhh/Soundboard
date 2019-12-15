using NAudio.Wave;
using System;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Represents an audio device for input devices.
    /// </summary>
    public class WaveInDevice : IAudioDevice, IEquatable<WaveInDevice>
    {

        public WaveInDevice(WaveInCapabilities capabilities, int number, string friendlyName)
        {
            Capabilities = capabilities;
            DeviceId = number;
            FriendlyName = friendlyName;
        }

        public WaveInCapabilities Capabilities { get; }

        public string FriendlyName { get; }

        public int DeviceId { get; }

        public bool Equals(WaveInDevice other)
        {
            return Capabilities.Equals(other.Capabilities);
        }

        public bool Equals(IAudioDevice other)
        {
            if (!(other is WaveInDevice)) return false;

            return Equals((WaveInDevice)other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is WaveInDevice)) return false;

            return Equals((WaveInDevice)obj);
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
