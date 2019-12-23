using vm = ManiacSoundboard.ViewModel;
using System.Windows;
using System.IO;
using System;
using ManiacSoundboard.ViewModel;
using Microsoft.Shell;
using System.Collections.Generic;

namespace ManiacSoundboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {

        private const string Unique = "Maniac Soundboard";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // Bring window to foreground
            if (this.MainWindow.WindowState == WindowState.Minimized)
            {
                this.MainWindow.WindowState = WindowState.Normal;
            }

            this.MainWindow.Activate();

            return true;
        }

        #endregion

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
                var result = vm.AppServices.MessageBoxService.ShowMessageBoxDecision("Some problem occurred when application was starting to run. Do you want to see the details?",
                                                                        "Problem with startup.", vm.MessageBoxButton.YesNo, vm.MessageBoxImage.Error);

                if (result == vm.MessageBoxResult.Yes)
                    vm.AppServices.MessageBoxService.ShowMessageBox(ex.Message, "Error details", vm.MessageBoxImage.Information);

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
