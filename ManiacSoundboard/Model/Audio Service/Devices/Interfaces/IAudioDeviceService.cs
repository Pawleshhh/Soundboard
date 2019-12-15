using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacSoundboard.Model
{
    public interface IAudioDeviceService : IDisposable
    {

        IDeviceNotification Notifications { get; }

        IReadOnlyCollection<IAudioDevice> OutDevices { get; }

        IReadOnlyCollection<IAudioDevice> InDevices { get; }

        void ReloadDevices();

        void ReloadOutDevices();

        void ReloadInDevices();

    }
}
