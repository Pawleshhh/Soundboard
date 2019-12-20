using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacSoundboard.ViewModel
{

    /// <summary>
    /// Stores all app services.
    /// </summary>
    public static class AppServices
    {

        #region MessageBoxService

        private static IMessageBoxService _messageBoxService;

        public static IMessageBoxService MessageBoxService
        {
            get
            {
                if (_messageBoxService == null)
                    _messageBoxService = GetMessageBoxService();

                return _messageBoxService;
            }
        }

        public static IMessageBoxService GetMessageBoxService()
        {
            string lowered = AppConfiguration.Configurations["UIFramework"].ToLowerInvariant();

            switch (lowered)
            {
                case "wpf":
                    return new WpfAppMessageBox(App.Current.MainWindow);
                default:
                    throw new InvalidOperationException("Not recognized type of UIFramework");
            }
        }

        #endregion

        #region FileFolderDialogService

        private static IFileFolderDialogService _fileFolderDialogService;

        public static IFileFolderDialogService FileFolderDialogService
        {
            get
            {
                if (_fileFolderDialogService == null)
                    _fileFolderDialogService = GetFileFolderDialogService();

                return _fileFolderDialogService;
            }
        }

        public static IFileFolderDialogService GetFileFolderDialogService()
        {
            string lowered = AppConfiguration.Configurations["UIFramework"].ToLowerInvariant();

            switch (lowered)
            {
                case "wpf":
                    return new WpfAppFileFolderDialog()
                    {
                        Filter = FileFolderHelper.GetWindowsExplorerFilters(RecognizableFormats.GetAllFormats().Distinct(new DistinctFileFormatComparer()).ToArray())
                    };
                default:
                    throw new InvalidOperationException("Not recognized type of UIFramework");
            }
        }

        #endregion

    }
}
