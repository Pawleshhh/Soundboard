using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacSoundboard.ViewModel
{
    public static class AppConfiguration
    {

        static AppConfiguration()
        {
            Configurations = XmlSerializationService.ReadDataFromXml(Properties.Resources.Configuration, "Configuration");
        }

        public static IReadOnlyDictionary<string, string> Configurations { get; private set; }

    }
}
