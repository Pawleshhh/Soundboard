
using AudioSwitcher.AudioApi;
using System;

namespace ManiacSoundboard.Model
{
    public class DeviceNotificationEventArgs : EventArgs
    {

        public string DeviceId { get; }

        public DeviceNotificationEventArgs(string deviceId)
        {
            DeviceId = deviceId;
        }

    }
}
