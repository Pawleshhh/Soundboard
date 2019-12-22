using ManiacSoundboard.Model;

namespace ManiacSoundboard.ViewModel
{

    /// <summary>
    /// Simple singleton factory to create specified implementations of <see cref="SoundboardViewModel"/>.
    /// </summary>
    public class SoundboardViewModelSimpleFactory
    {

        private SoundboardViewModelSimpleFactory() { }

        private static SoundboardViewModelSimpleFactory _factory;

        public static SoundboardViewModelSimpleFactory Factory
        {
            get
            {
                if (_factory == null)
                    _factory = new SoundboardViewModelSimpleFactory();

                return _factory;
            }
        }

        public SoundboardViewModel GetSoundboardViewModel(string soundboardType, Soundboard soundboard, IMessageBoxService messageBoxService, IFileFolderDialogService fileFolderDialogService)
        {
            if (string.IsNullOrWhiteSpace(soundboardType))
                return new SoundboardViewModel(soundboard, messageBoxService, fileFolderDialogService);

            string loweredType = soundboardType.ToLowerInvariant();

            switch(loweredType)
            {
                case "keyboard":
                    return new SoundboardKeyboardViewModel(soundboard, messageBoxService, fileFolderDialogService);

                default:
                    return new SoundboardViewModel(soundboard, messageBoxService, fileFolderDialogService);
            }
        }

        public SoundboardViewModel GetSoundboardViewModel(string soundboardType)
        {
            if (string.IsNullOrWhiteSpace(soundboardType))
                return new SoundboardViewModel();

            string loweredType = soundboardType.ToLowerInvariant();

            switch (loweredType)
            {
                case "keyboard":
                    return new SoundboardKeyboardViewModel();

                default:
                    return new SoundboardViewModel();
            }
        }

        public Soundboard GetConfiguredModel()
        {
            AudioOutputApi api = XmlHelper.Get<AudioOutputApi>(AppConfiguration.Configurations["AudioOutputApi"]);

            return SoundboardStaticSimpleFactory.GetSoundboard(api);
        }

    }
}
