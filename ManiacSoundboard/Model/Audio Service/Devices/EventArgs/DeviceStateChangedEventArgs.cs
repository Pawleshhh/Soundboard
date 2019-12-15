using NAudio.CoreAudioApi;
using System;

namespace ManiacSoundboard.Model
{
    public class DeviceStateChangedEventArgs : DeviceNotificationEventArgs
    {

        public DeviceState State { get; }

        public DeviceStateChangedEventArgs(string deviceId, DeviceState state) : base(deviceId)
        {
            State = state;
        }

    }
}
