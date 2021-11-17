using Kingmaker.PubSubSystem;

namespace QuickCast.UI.QuickInventory
{
    public interface IViewChangeHandler : ISubscriber, IGlobalSubscriber
    {
        void HandleViewChange();
    }
}
