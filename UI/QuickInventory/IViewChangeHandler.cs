using Kingmaker.PubSubSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WIT.UI.QuickInventory.MainWindowManager;

namespace WIT.UI.QuickInventory
{
    public interface IViewChangeHandler : ISubscriber, IGlobalSubscriber
    {
        void HandleViewChange();
    }
}
