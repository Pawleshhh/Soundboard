using vm = ManiacSoundboard.ViewModel;
using System.Windows;
using System.IO;
using System;

namespace ManiacSoundboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private IViewModel _mainViewModel;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            MainWindow = mainWindow;

            try
            {
                if (File.Exists(Environment.CurrentDirectory + "\\" + "soundboardData.xml"))
                    MainWindow.DataContext = _mainViewModel = vm.XmlSerializationService.Deserialize<vm.MainViewModel>("soundboardData.xml");
                else
                    MainWindow.DataContext = _mainViewModel = new vm.MainViewModel();
            }
            catch (Exception ex)
            {
                var result = vm.AppServices.MessageBoxService.ShowMessageBoxDecision("soundboardData.xml file is not properly written so saved settings and data weren't loaded. Do you want to see the details?",
                                                                        "Loading data from file error", vm.MessageBoxButton.YesNo, vm.MessageBoxImage.Error);

                if (result == vm.MessageBoxResult.Yes)
                    vm.AppServices.MessageBoxService.ShowMessageBox(ex.Message, "Details", vm.MessageBoxImage.Information);

                _mainViewModel?.Dispose();

                MainWindow.DataContext = _mainViewModel = new vm.MainViewModel();
            }

            mainWindow.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _mainViewModel?.WhenClosing();
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            _mainViewModel?.WhenClosing();
        }

    }
}
