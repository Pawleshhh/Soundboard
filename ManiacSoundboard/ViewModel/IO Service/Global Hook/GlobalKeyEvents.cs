using System;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace ManiacSoundboard.ViewModel
{

    public class GlobalKeyEvents : IDisposable
    {

        #region Constructors

        public GlobalKeyEvents()
        {
            _globalHook = Hook.GlobalEvents();

            _globalHook.KeyDown += _globalHook_KeyDown;
            _globalHook.KeyUp += _globalHook_KeyUp;
            _globalHook.KeyPress += _globalHook_KeyPress;
        }

        ~GlobalKeyEvents()
        {
            Dispose();
        }

        #endregion

        #region Private fields

        private bool _disposed;

        private IKeyboardMouseEvents _globalHook;

        #endregion

        #region Properties

        #endregion

        #region Events

        public event EventHandler<KeyEventArgsExt> KeyDown;

        public event EventHandler<KeyEventArgsExt> KeyUp;

        public event EventHandler<KeyPressEventArgsExt> KeyPress;

        protected void OnKeyDown(KeyEventArgsExt e) => KeyDown?.Invoke(this, e);

        protected void OnKeyUp(KeyEventArgsExt e) => KeyUp?.Invoke(this, e);

        protected void OnKeyPress(KeyPressEventArgsExt e) => KeyPress?.Invoke(this, e);

        #endregion

        #region Methods

        public void Dispose()
        {
            if (_disposed) return;

            _globalHook.KeyDown -= _globalHook_KeyDown;
            _globalHook.KeyUp -= _globalHook_KeyUp;
            _globalHook.KeyPress -= _globalHook_KeyPress;
            _globalHook.Dispose();

            _disposed = true;

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private methods

        private void _globalHook_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnKeyPress(new KeyPressEventArgsExt(e.KeyChar));
        }

        private void _globalHook_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(new KeyEventArgsExt(e.KeyData));
        }

        private void _globalHook_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(new KeyEventArgsExt(e.KeyData));
        }

        #endregion

    }

}
