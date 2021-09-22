using Kingmaker;
using Kingmaker.PubSubSystem;
using ModMaker;
using ModMaker.Utility;
using UnityEngine;
using static WIT.Main;
using WIT.UI.QuickInventory;

namespace WIT.Controllers
{
    internal class QuickInvUIController : IModEventHandler, IAreaLoadingStagesHandler
    {
        public int Priority => 400;

        public MainWindowManager MainWindowManager { get; private set; }

        public void Attach()
        {
            if (MainWindowManager == null)
                MainWindowManager = MainWindowManager.CreateObject();
        }

        public void Detach()
        {
            MainWindowManager.SafeDestroy();
            MainWindowManager = null;
        }

        public void Update()
        {
            Detach();
            Attach();
        }

#if DEBUG

        public void Clear()
        {
            Transform quickInventory;
            while (quickInventory = Game.Instance.UI.Common.transform.Find("QuickInventory"))
            {
                quickInventory.SafeDestroy();
            }
            quickInventory = null;
        }

#endif

        public void HandleModEnable()
        {
            Mod.Core.UI = this;
            EventBus.Subscribe(this);
        }

        public void HandleModDisable()
        {
            EventBus.Unsubscribe(this);
            Detach();
            Mod.Core.UI = null;
        }

        public void OnAreaScenesLoaded()
        {
        }

        public void OnAreaLoadingComplete()
        {
            Attach();
        }
    }
}