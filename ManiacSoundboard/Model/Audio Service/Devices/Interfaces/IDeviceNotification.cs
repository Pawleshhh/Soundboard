using NAudio.CoreAudioApi.Interfaces;
using System;

namespace ManiacSoundboard.Model
{
    public interface IDeviceNotification
    {

        event EventHandler<DeviceNotificationEventArgs> DefaultDeviceChaned;

        event EventHandler<DeviceNotificationEventArgs> DeviceAdded;

        event EventHandler<DeviceNotificationEventArgs> DeviceRemoved;

        event EventHandler<DeviceStateChangedEventArgs> DeviceStateChanged;

        event EventHandler<DeviceNotificationEventArgs> PropertyValueChanged;

    }
}
