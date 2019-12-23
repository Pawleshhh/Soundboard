using AudioSwitcher.AudioApi.CoreAudio;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Contains information about the audio devices of this local device.
    /// </summary>
    public class WaveDevices : IAudioDeviceService
    {

        #region Constructors

        public WaveDevices()
        {
            _enumerator = new MMDeviceEnumerator();
            //Notifications = new MMRenderNotificationClient();
            //RegisterEndpointNotificationCallback((MMRenderNotificationClient)Notifications);
        }

        #endregion

        #region Private fields

        private MMDeviceEnumerator _enumerator;

        #endregion

        #region Properties

        /// <summary>
        /// Gets object that notifies if devices changed.
        /// </summary>
        public IDeviceNotification Notifications => throw new NotImplementedException("Notifications are not used.");

        /// <summary>
        /// Gets collection of stored in (capture) audio devices.
        /// </summary>
        public IReadOnlyCollection<IAudioDevice> InDevices { get; private set; }

        /// <summary>
        /// Gets collection of stored out (render) audio devices.
        /// </summary>
        public IReadOnlyCollection<IAudioDevice> OutDevices { get; private set; }

        #endregion

        #region Methods

        /*
        public bool WaveOutDeviceExists(WaveOutDevice device)
        {
            foreach (var outDevice in WaveOutDevices)
                if (outDevice.Equals(device)) return true;

            return false;
        }

        public bool WaveInDeviceExists(WaveInDevice device)
        {
            foreach (var inDevice in WaveInDevices)
                if (inDevice.Equals(device)) return true;

            return false;
        }

        public bool WaveDeviceExists(IWaveDevice device)
        {
            if (device is WaveOutDevice outDev)
            {
                if (WaveOutDeviceExists(outDev)) return true;
            }
            else if (device is WaveInDevice inDev)
            {
                if (WaveInDeviceExists(inDev)) return true;
            }

            return false;
        }*/

        /// <summary>
        /// Updates collections of out (<see cref="OutDevices"/>) and in (<see cref="InDevices"/>) devices.
        /// </summary>
        public void ReloadDevices()
        {
            InDevices = _GetWaveInDevices();
            OutDevices = _GetWaveOutDevices();
        }

        /// <summary>
        /// Updates collection of out (<see cref="OutDevices"/>).
        /// </summary>
        public void ReloadOutDevices()
        {
            OutDevices = _GetWaveOutDevices();
        }

        /// <summary>
        /// Updates collection of in (<see cref="InDevices"/>).
        /// </summary>
        public void ReloadInDevices()
        {
            InDevices = _GetWaveInDevices();
        }

        /// <summary>
        /// Checks whether the given device exists, is available or not.
        /// </summary>
        /// <param name="device">Device to be checked.</param>
        public bool DeviceExists(IAudioDevice device)
        {
            if (device == null) return false;

            MMDeviceCollection mmDeviceCollection;

            if (device is WaveOutDevice)
            {
                mmDeviceCollection = _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            }
            else if (device is WaveInDevice)
            {
                mmDeviceCollection = _enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            }
            else return false;


            foreach(var mmDevice in mmDeviceCollection)
            {
                if (device.FriendlyName.StartsWith(mmDevice.FriendlyName))
                    return true;
            }


            return false;
        }

        public void Dispose()
        {
            _enumerator.Dispose();
            //if (Notifications != null)
            //{
            //    UnRegisterEndpointNotificationCallback((MMRenderNotificationClient)Notifications);
            //}
        }

        private WaveOutDevice[] _GetWaveOutDevices()
        {
            try
            {
                int outDevicesCount = WaveOut.DeviceCount;
                MMDeviceCollection mmDeviceCollection = _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

                WaveOutDevice[] waveOutDevices = new WaveOutDevice[outDevicesCount];

                for (int outDevice = 0; outDevice < outDevicesCount; outDevice++)
                {
                    var capabilities = WaveOut.GetCapabilities(outDevice);
                    //waveOutDevices[outDevice] = new WaveOutDevice(capabilities, outDevice);

                    //Getting full names of the devices.
                    foreach (MMDevice device in mmDeviceCollection)
                    {
                        if (device.FriendlyName.StartsWith(capabilities.ProductName))
                        {
                            waveOutDevices[outDevice] = new WaveOutDevice(capabilities, outDevice, device.FriendlyName);
                            break;
                        }
                    }
                }

                return waveOutDevices;
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Error occurred when getting wave out devices. {0}", ex.Message), ex);
            }
        }

        private WaveInDevice[] _GetWaveInDevices()
        {
            try
            {
                int inDevicesCount = WaveIn.DeviceCount;
                MMDeviceCollection mmDeviceCollection = _enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

                WaveInDevice[] waveInDevices = new WaveInDevice[inDevicesCount];

                for (int inDevice = 0; inDevice < inDevicesCount; inDevice++)
                {
                    var capabilities = WaveIn.GetCapabilities(inDevice);
                    //waveInDevices[inDevice] = new WaveInDevice(capabilities, inDevice);

                    //Getting full names of the devices.
                    foreach (MMDevice device in mmDeviceCollection)
                    {
                        if (device.FriendlyName.StartsWith(capabilities.ProductName))
                        {
                            waveInDevices[inDevice] = new WaveInDevice(capabilities, inDevice, device.FriendlyName);
                            break;
                        }
                    }
                }

                return waveInDevices;
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Error occurred when getting wave in devices. {0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// Registers a call back for Device Events
        /// </summary>
        /// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface</param>
        /// <returns></returns>
        private int RegisterEndpointNotificationCallback([In] [MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
        {
            return _enumerator.RegisterEndpointNotificationCallback(client);
        }

        /// <summary>
        /// UnRegisters a call back for Device Events
        /// </summary>
        /// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface </param>
        /// <returns></returns>
        private int UnRegisterEndpointNotificationCallback([In] [MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
        {
            return _enumerator.UnregisterEndpointNotificationCallback(client);
        }

        #endregion

    }
}
