
using AudioSwitcher.AudioApi;
using System;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Stores event arguments for <see cref="IDeviceNotification"/> events.
    /// </summary>
    public class DeviceNotificationEventArgs : EventArgs
    {

        /// <summary>
        /// Gets device's id that has been changed.
        /// </summary>
        public string DeviceId { get; }

        public DeviceNotificationEventArgs(string deviceId)
        {
            DeviceId = deviceId;
        }

    }
}
