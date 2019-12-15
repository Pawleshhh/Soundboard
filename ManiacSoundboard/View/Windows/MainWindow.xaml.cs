using ManiacSoundboard.ViewModel;
using System.Windows;

namespace ManiacSoundboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : AppBaseWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void soundboardListView_Drop(object sender, DragEventArgs e)
        {
            MainViewModel mainViewModel = (MainViewModel)DataContext;

            if (mainViewModel.SoundboardViewModel.IsNotBusy && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                mainViewModel.SoundboardViewModel.AddSoundsByPaths(files);
            }
        }

        //private void MainWindow_StateChanged(object sender, EventArgs e)
        //{
        //    if(WindowState == WindowState.Maximized)
        //    {
        //        this.Left = SystemParameters.WorkArea.Left;
        //        this.Top = SystemParameters.WorkArea.Top;
        //        this.Height = SystemParameters.WorkArea.Height;
        //        this.Width = SystemParameters.WorkArea.Width;
        //    }
        //}
    }
}
