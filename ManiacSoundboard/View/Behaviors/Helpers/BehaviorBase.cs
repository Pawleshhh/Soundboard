using System;
using System.Windows;
using System.Windows.Interactivity;

namespace ManiacSoundboard
{

    /// <summary>
    /// Base class for all behaviors used in this application.
    /// </summary>
    /// <typeparam name="T">Type of specified <see cref="FrameworkElement"/>.</typeparam>
    public abstract class BehaviorBase<T> : Behavior<T> where T : FrameworkElement
    {
        private bool _isSetup = true;
        private bool _isHookedUp;
        private WeakReference _weakTarget;

        protected virtual void OnSetup() { }
        protected virtual void OnCleanup() { }
        protected override void OnChanged()
        {
            var target = AssociatedObject;
            if (target != null)
            {
                HookupBehavior(target);
            }
            else
            {
                UnHookupBehavior();
            }
        }

        private void OnTarget_Loaded(object sender, RoutedEventArgs e) { SetupBehavior(); }

        private void OnTarget_Unloaded(object sender, RoutedEventArgs e) { CleanupBehavior(); }

        private void HookupBehavior(T target)
        {
            if (_isHookedUp) return;
            _weakTarget = new WeakReference(target);
            _isHookedUp = true;
            target.Unloaded += OnTarget_Unloaded;
            target.Loaded += OnTarget_Loaded;
            SetupBehavior();
        }

        private void UnHookupBehavior()
        {
            if (!_isHookedUp) return;
            _isHookedUp = false;
            var target = AssociatedObject ?? (T)_weakTarget.Target;
            if (target != null)
            {
                target.Unloaded -= OnTarget_Unloaded;
                target.Loaded -= OnTarget_Loaded;
            }
            CleanupBehavior();
        }

        private void SetupBehavior()
        {
            if (_isSetup) return;
            _isSetup = true;
            OnSetup();
        }

        private void CleanupBehavior()
        {
            if (!_isSetup) return;
            _isSetup = false;
            OnCleanup();
        }
    }
}
