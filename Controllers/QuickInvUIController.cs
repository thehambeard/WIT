using Kingmaker;
using Kingmaker.PubSubSystem;
using ModMaker;
using ModMaker.Utility;
using QuickCast.UI.QuickInventory;
using QuickCast.Utilities;
using UnityEngine;
using static QuickCast.Main;

namespace QuickCast.Controllers
{
    internal class QuickInvUIController : IModEventHandler, IAreaLoadingStagesHandler, IAreaHandler
    {
        public int Priority => 200;

        public MainWindowManager MainWindowManager { get; private set; }

        public void Attach()
        {
            if (MainWindowManager == null)
                MainWindowManager = MainWindowManager.CreateObject();
        }

        public void Detach()
        {
            EventBus.Unsubscribe(MainWindowManager);
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
            SetWrap.Window_Scale = MainWindowManager.transform.localScale;
            SetWrap.Window_Pos = MainWindowManager.transform.localPosition;
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

        public void OnAreaBeginUnloading()
        {
            Detach();
        }

        public void OnAreaDidLoad()
        {
        }
    }
}