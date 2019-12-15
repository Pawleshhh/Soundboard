using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacSoundboard.ViewModel
{
    public interface IMainThreadWorker
    {

        void WorkOnMainThread(Action action);

    }
}
