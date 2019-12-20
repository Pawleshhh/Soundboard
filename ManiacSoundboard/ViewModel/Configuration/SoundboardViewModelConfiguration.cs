using ManiacSoundboard.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManiacSoundboard.ViewModel
{

    /// <summary>
    /// Creates objects needed to create configured <see cref="SoundboardViewModel"/>. This class is made in singleton pattern.
    /// </summary>
    public class SoundboardViewModelConfiguration
    {

        private static SoundboardViewModelConfiguration configurationService;

        public static SoundboardViewModelConfiguration ConfigurationService
        {
            get
            {
                if (configurationService == null)
                    configurationService = new SoundboardViewModelConfiguration();

                return configurationService;
            }
        }

        public SoundboardViewModel GetConfiguredViewModel()
        {
            try
            {
                Soundboard soundboard = GetConfiguredModel();

                var soundboardVM = SoundboardViewModelSimpleFactory.Factory.
                                            GetSoundboardViewModel(AppConfiguration.Configurations["Interaction"], soundboard, 
                                                                    AppServices.MessageBoxService, AppServices.FileFolderDialogService);

                return soundboardVM;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not load settings from configuration file.", ex);
            }
        }

        public SoundboardViewModel GetSoundboardViewModel()
        {
            return SoundboardViewModelSimpleFactory.Factory.GetSoundboardViewModel(AppConfiguration.Configurations["Interaction"]);
        }

        public Soundboard GetConfiguredModel()
        {
            AudioOutputApi api = XmlHelper.Get<AudioOutputApi>(AppConfiguration.Configurations["AudioOutputApi"]);

            return SoundboardStaticSimpleFactory.GetSoundboard(api);
        }
    }
}
