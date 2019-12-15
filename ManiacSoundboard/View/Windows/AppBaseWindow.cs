using System;
using System.Drawing;
using System.Windows;
using forms = System.Windows.Forms;

namespace ManiacSoundboard
{
    public class AppBaseWindow : Window
    {

        #region Constructors

        public AppBaseWindow()
        {
            Closed += AppBaseWindow_Closed;
        }

        #endregion

        #region Private fields

        private bool _notifyIconDisposed;

        private forms.NotifyIcon _notifyIcon;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty TrayIconProperty =
            DependencyProperty.Register("TrayIcon", typeof(string), typeof(AppBaseWindow),
                new PropertyMetadata(OnTrayIconChanged));

        public static readonly DependencyProperty IsTrayIconEnabledProperty =
            DependencyProperty.Register("IsTrayIconEnabled", typeof(bool), typeof(AppBaseWindow),
                new PropertyMetadata(false, OnIsTrayEnabledChanged));

        public static readonly DependencyProperty TrayTitleProperty =
            DependencyProperty.Register("TrayTitle", typeof(string), typeof(AppBaseWindow),
                new PropertyMetadata(string.Empty, OnTrayTitleChanged));

        public string TrayTitle
        {
            get { return (string)GetValue(TrayTitleProperty); }
            set { SetValue(TrayTitleProperty, value); }
        }

        public string TrayIcon
        {
            get { return (string)GetValue(TrayIconProperty); }
            set { SetValue(TrayIconProperty, value); }
        }

        public bool IsTrayIconEnabled
        {
            get { return (bool)GetValue(IsTrayIconEnabledProperty); }
            set { SetValue(IsTrayIconEnabledProperty, value); }
        }

        #endregion

        #region Properties

        public forms.NotifyIcon NotifyIcon
        {
            get
            {
                if(_notifyIcon == null || _notifyIconDisposed)
                {
                    _InitializeNotifyIcon();
                }

                return _notifyIcon;
            }
            private set => _notifyIcon = value;
        }

        #endregion

        #region Methods

        public void SwitchWindowState()
        {
            if (WindowState == WindowState.Minimized)
                ShowWindow();
            else
                HideWindow();
        }

        public void ShowWindow()
        {
            Show();
            WindowState = WindowState.Normal;
        }

        public void HideWindow()
        {
            Hide();
            WindowState = WindowState.Minimized;
        }

        private void _InitializeNotifyIcon()
        {
            string path = TrayIcon;

            NotifyIcon = new forms.NotifyIcon();

            if (string.IsNullOrWhiteSpace(path))
                NotifyIcon.Icon = Properties.Resources.soundboardIcon_1;
            else
                NotifyIcon.Icon = new Icon(path);

            NotifyIcon.Visible = IsTrayIconEnabled;
            NotifyIcon.Text = TrayTitle;

            NotifyIcon.Disposed += NotifyIcon_Disposed;

            NotifyIcon.MouseDoubleClick += (sender, _e) =>
            {
                if (_e.Button != forms.MouseButtons.Left) return;

                SwitchWindowState();
            };

            forms.MenuItem minimizeMenuItem = new forms.MenuItem();
            minimizeMenuItem.Text = "Minimize";
            minimizeMenuItem.Click += (sender, _e) =>
            {
                HideWindow();
            };

            forms.MenuItem maximizeMenuItem = new forms.MenuItem();
            maximizeMenuItem.Text = "Maximize";
            maximizeMenuItem.Click += (sender, _e) =>
            {
                ShowWindow();
            };

            forms.MenuItem closeMenuItem = new forms.MenuItem();
            closeMenuItem.Text = "Close";
            closeMenuItem.Click += (sender, _e) =>
            {
                Close();
            };

            NotifyIcon.ContextMenu = new forms.ContextMenu(new forms.MenuItem[] { maximizeMenuItem, minimizeMenuItem, closeMenuItem });
        }

        private void NotifyIcon_Disposed(object sender, EventArgs e)
        {
            _notifyIconDisposed = true;
        }

        private void AppBaseWindow_Closed(object sender, EventArgs e)
        {
            if (!_notifyIconDisposed)
                NotifyIcon.Dispose();
        }

        #endregion

        #region Static methods

        private static void OnTrayIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AppBaseWindow window = d as AppBaseWindow;
            if (window == null) return;

            string newValue = (string)e.NewValue;
            string path = string.IsNullOrWhiteSpace(newValue) ? (string)App.Current.Resources["soundboardIcon_1.png"] : newValue;
            window.NotifyIcon.Icon = new Icon(path);
        }

        private static void OnIsTrayEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AppBaseWindow appBaseWindow = d as AppBaseWindow;
            if (appBaseWindow == null) return;

            bool value = (bool)e.NewValue;

            if (value)
                appBaseWindow._InitializeNotifyIcon();
        }

        private static void OnTrayTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AppBaseWindow appBaseWindow = d as AppBaseWindow;
            if (appBaseWindow == null) return;

            string value = (string)e.NewValue;

            appBaseWindow.NotifyIcon.Text = value;
        }

        #endregion

    }
}
