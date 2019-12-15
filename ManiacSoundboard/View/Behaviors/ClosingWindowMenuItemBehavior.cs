using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace ManiacSoundboard
{

    /// <summary>
    /// Behavior of closing the window by a menu item control.
    /// </summary>
    public class ClosingWindowMenuItemBehavior : Behavior<MenuItem>
    {

        /// <summary>
        /// Contains reference to the window to be closed.
        /// </summary>
        public static readonly DependencyProperty WindowToCloseProperty = DependencyProperty.Register("WindowToClose", typeof(Window), typeof(ClosingWindowMenuItemBehavior));

        /// <summary>
        /// Gets window to be closed.
        /// </summary>
        public Window WindowToClose
        {
            get => (Window)GetValue(WindowToCloseProperty);
            set => SetValue(WindowToCloseProperty, value);
        }

        protected override void OnAttached()
        {
            MenuItem menuItem = AssociatedObject;
            
            if(menuItem != null)
                menuItem.Click += MenuItem_Click;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            WindowToClose?.Close();
        }
    }
}
