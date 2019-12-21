using System;
using System.Collections.Generic;
using System.Configuration;
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
            //Dictionary<string, string> config = new Dictionary<string, string>();
            //config.Add("UIFramework", ConfigurationManager.AppSettings["UIFramework"]);
            //config.Add("AudioOutputApi", ConfigurationManager.AppSettings["AudioOutputApi"]);
            //config.Add("Interaction", ConfigurationManager.AppSettings["Interaction"]);

            //Configurations = config;
        }

        /// <summary>
        /// Gets dictionary of configurations.
        /// </summary>
        public static IReadOnlyDictionary<string, string> Configurations { get; private set; }

    }
}
