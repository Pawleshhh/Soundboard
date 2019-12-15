using ManiacSoundboard.ViewModel;
using System.Windows;
using ManiacSoundboard.Properties;
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

            if (File.Exists(Environment.CurrentDirectory + "\\" + "soundboardData.xml"))
                MainWindow.DataContext = _mainViewModel = XmlSerializationService.Deserialize<MainViewModel>("soundboardData.xml");
            else
                MainWindow.DataContext = _mainViewModel = new MainViewModel();

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
