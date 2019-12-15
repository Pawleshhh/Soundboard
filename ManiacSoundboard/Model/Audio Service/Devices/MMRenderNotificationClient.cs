using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;

namespace ManiacSoundboard.Model
{
    public class MMRenderNotificationClient : IDeviceNotification, IMMNotificationClient
    {

        public event EventHandler<DeviceNotificationEventArgs> DefaultDeviceChaned;

        public event EventHandler<DeviceNotificationEventArgs> DeviceAdded;

        public event EventHandler<DeviceNotificationEventArgs> DeviceRemoved;

        public event EventHandler<DeviceStateChangedEventArgs> DeviceStateChanged;

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
