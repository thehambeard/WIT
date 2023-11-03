using Kingmaker.PubSubSystem;
using QuickCast.UI.Monos;
using QuickCast.Utility;
using QuickCast.Utility.Extentions;

namespace QuickCast.UI.Controllers
{
    internal class MainUIController : IModEventHandler, IAreaLoadingStagesHandler, IAreaHandler
    {

        public static MainUIController Instance { get; private set; }
        public MainUIManager MainUIManager { get; private set; }

        public int Priority => 100;

        public void Bind()
        {
            if (MainUIManager == null)
                MainUIManager = Builders.BuildUI.BuildMainUI();
        }

        public void Unbind()
        {
            MainUIManager.SafeDestroy();
        }

        public void OnAreaBeginUnloading() => Unbind();
        public void OnAreaLoadingComplete() => Bind();
        public void OnAreaDidLoad() { }
        public void OnAreaScenesLoaded() { }

        public void HandleModEnable()
        {
            EventBus.Subscribe(this);
            Instance = this;
        }

        public void HandleModDisable()
        {
            EventBus.Unsubscribe(this);
            Instance = null;
        }
    }
}
