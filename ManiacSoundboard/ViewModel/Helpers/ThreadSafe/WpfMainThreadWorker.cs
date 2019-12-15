using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacSoundboard.ViewModel
{
    public class WpfMainThreadWorker : IMainThreadWorker
    {
        public void WorkOnMainThread(Action action)
        {
            App.Current.Dispatcher.Invoke(action);
        }
    }
}
