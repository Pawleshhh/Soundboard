using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacSoundboard.Model
{

    /// <summary>
    /// Defines default interface for all services that stores infos about current audio devices.
    /// </summary>
    public interface IAudioDeviceService : IDisposable
    {

        /// <summary>
        /// Gets object that notifies if devices changed.
        /// </summary>
        IDeviceNotification Notifications { get; }

        /// <summary>
        /// Gets collection of stored out (render) audio devices.
        /// </summary>
        IReadOnlyCollection<IAudioDevice> OutDevices { get; }

        /// <summary>
        /// Gets collection of stored in (capture) audio devices.
        /// </summary>
        IReadOnlyCollection<IAudioDevice> InDevices { get; }

        /// <summary>
        /// Updates collections of out (<see cref="OutDevices"/>) and in (<see cref="InDevices"/>) devices.
        /// </summary>
        void ReloadDevices();

        /// <summary>
        /// Updates collection of out (<see cref="OutDevices"/>).
        /// </summary>
        void ReloadOutDevices();

        /// <summary>
        /// Updates collection of in (<see cref="InDevices"/>).
        /// </summary>
        void ReloadInDevices();

        /// <summary>
        /// Checks whether the given device exists, is available or not.
        /// </summary>
        /// <param name="device">Device to be checked.</param>
        bool DeviceExists(IAudioDevice device);

    }
}
