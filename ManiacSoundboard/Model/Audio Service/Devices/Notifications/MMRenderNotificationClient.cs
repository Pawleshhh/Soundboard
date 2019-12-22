using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Gives events that notify that render audio devices changed (e.g. default render device changed, plugged, unplugged, etc.).
    /// </summary>
    public class MMRenderNotificationClient : IDeviceNotification, IMMNotificationClient
    {

        /// <summary>
        /// Rises when default device changed.
        /// </summary>
        public event EventHandler<DeviceNotificationEventArgs> DefaultDeviceChaned;

        /// <summary>
        /// Rises when device has been added.
        /// </summary>
        public event EventHandler<DeviceNotificationEventArgs> DeviceAdded;

        /// <summary>
        /// Rises when device has been removed.
        /// </summary>
        public event EventHandler<DeviceNotificationEventArgs> DeviceRemoved;

        /// <summary>
        /// Rises when device's state has been changed.
        /// </summary>
        public event EventHandler<DeviceStateChangedEventArgs> DeviceStateChanged;

        /// <summary>
        /// Rises when device's property has been changed.
        /// </summary>
        public event EventHandler<DeviceNotificationEventArgs> PropertyValueChanged;

        void IMMNotificationClient.OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            if(flow == DataFlow.Render && role == Role.Multimedia)
                DefaultDeviceChaned?.Invoke(this, new DeviceNotificationEventArgs(defaultDeviceId));
        }

        void IMMNotificationClient.OnDeviceAdded(string pwstrDeviceId)
        {
            DeviceAdded?.Invoke(this, new DeviceNotificationEventArgs(pwstrDeviceId));
        }

        void IMMNotificationClient.OnDeviceRemoved(string deviceId)
        {
            DeviceRemoved?.Invoke(this, new DeviceNotificationEventArgs(deviceId));
        }

        void IMMNotificationClient.OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            DeviceStateChanged?.Invoke(this, new DeviceStateChangedEventArgs(deviceId, newState));
        }

        void IMMNotificationClient.OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {
            PropertyValueChanged?.Invoke(this, new DeviceNotificationEventArgs(pwstrDeviceId));
        }

    }
}
