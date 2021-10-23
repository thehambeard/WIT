using Kingmaker.PubSubSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QuickCast.UI.QuickInventory.MainWindowManager;

namespace QuickCast.UI.QuickInventory
{
    public interface IViewChangeHandler : ISubscriber, IGlobalSubscriber
    {
        void HandleViewChange();
    }
}
