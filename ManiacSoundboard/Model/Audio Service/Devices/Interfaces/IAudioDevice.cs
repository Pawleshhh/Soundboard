using System;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Interface of audio devices.
    /// </summary>
    public interface IAudioDevice : IEquatable<IAudioDevice>
    {

        /// <summary>
        /// Gets the number of the device.
        /// </summary>
        int DeviceId { get; }

        /// <summary>
        /// Gets friendly name of the device.
        /// </summary>
        string FriendlyName { get; }

    }

}
