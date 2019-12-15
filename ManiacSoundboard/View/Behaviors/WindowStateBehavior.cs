using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace ManiacSoundboard
{

    /// <summary>
    /// Class of the behavior for changing the window state.
    /// </summary>
    public class WindowStateBehavior : Behavior<Button>
    {

        /// <summary>
        /// Window state that the button sets on the window.
        /// </summary>
        public WindowButtonRole WindowButtonRole { get; set; } = WindowButtonRole.Normalize;

        private Window _window;

        protected override void OnAttached()
        {
            Button button = AssociatedObject;

            if (button != null) button.Click += Button_Click;
        }

        protected override void OnDetaching()
        {
            _window = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_window == null)
            {
                Window win = GetWindow();

                if (win == null) throw new InvalidOperationException("The button is not associated with any window!");

                ChangeWindowState(win);
                _window = win;
            }
            else
                ChangeWindowState(_window);
        }

        private void ChangeWindowState(Window window)
        {
            switch(WindowButtonRole)
            {
                case WindowButtonRole.Maximize:

                    if (window.WindowState == WindowState.Maximized)
                        window.WindowState = WindowState.Normal;
                    else
                    {
                        window.WindowState = WindowState.Maximized;
                        window.Height = SystemParameters.WorkArea.Height;
                    }

                    break;

                case WindowButtonRole.Minimize:

                    if (window.WindowState == WindowState.Minimized)
                        window.WindowState = WindowState.Normal;
                    else
                        window.WindowState = WindowState.Minimized;

                    break;

                case WindowButtonRole.Normalize:

                    window.WindowState = WindowState.Normal;

                    break;

                case WindowButtonRole.Close:

                    window.Close();

                    break;
            }
        }

        /// <summary>
        /// Searches for the window associated with the button.
        /// </summary>
        /// <returns>Returns the window associated with the button.</returns>
        private Window GetWindow()
        {
            var parent = VisualTreeHelper.GetParent(AssociatedObject);

            while (!(parent is Window) && parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return (Window)parent;
        }

    }

    /// <summary>
    /// Enum of roles of the buttons of changing window's state.
    /// </summary>
    public enum WindowButtonRole
    {
        Maximize, Minimize, Normalize, Close
    }

}
