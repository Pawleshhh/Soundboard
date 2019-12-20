using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacSoundboard.ViewModel
{

    /// <summary>
    /// Stores configurations of this application written in Configuration.xml file.
    /// </summary>
    public static class AppConfiguration
    {

        static AppConfiguration()
        {
            Configurations = XmlSerializationService.ReadDataFromXml(Properties.Resources.Configuration, "Configuration");
        }

        /// <summary>
        /// Gets dictionary of configurations.
        /// </summary>
        public static IReadOnlyDictionary<string, string> Configurations { get; private set; }

    }
}
