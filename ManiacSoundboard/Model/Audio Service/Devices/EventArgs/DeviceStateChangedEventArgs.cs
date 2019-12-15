using NAudio.CoreAudioApi;
using System;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Stores event arguments for <see cref="IDeviceNotification.DeviceStateChanged"/> event.
    /// </summary>
    public class DeviceStateChangedEventArgs : DeviceNotificationEventArgs
    {

        /// <summary>
        /// Gets changed state of given device.
        /// </summary>
        public DeviceState State { get; }

        public DeviceStateChangedEventArgs(string deviceId, DeviceState state) : base(deviceId)
        {
            State = state;
        }

    }
}
