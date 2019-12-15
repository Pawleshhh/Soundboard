using NAudio.CoreAudioApi.Interfaces;
using System;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Defines interface of objects that notify if audio devices changed.
    /// </summary>
    public interface IDeviceNotification
    {

        event EventHandler<DeviceNotificationEventArgs> DefaultDeviceChaned;

        event EventHandler<DeviceNotificationEventArgs> DeviceAdded;

        event EventHandler<DeviceNotificationEventArgs> DeviceRemoved;

        event EventHandler<DeviceStateChangedEventArgs> DeviceStateChanged;

        event EventHandler<DeviceNotificationEventArgs> PropertyValueChanged;

    }
}
