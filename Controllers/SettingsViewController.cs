using Kingmaker;
using Kingmaker.PubSubSystem;
using ModMaker;
using ModMaker.Utility;
using QuickCast.UI.QuickInventory;
using UnityEngine;
using static QuickCast.Main;
using static QuickCast.UI.QuickInventory.MainWindowManager;

namespace QuickCast.Controllers
{
    internal class SettingsViewController : IModEventHandler, IAreaLoadingStagesHandler, IAreaHandler
    {
        public int Priority => 400;

        public SettingsViewManager SettingsViewManage { get; private set; }

        public void Attach()
        {
            SettingsViewManage = SettingsViewManager.CreateObject(ViewPortType.Settings);
        }

        public void Detach()
        {
            if (SettingsViewManage != null)
            {
                EventBus.Unsubscribe(SettingsViewManage);
                SettingsViewManage.SafeDestroy();
                SettingsViewManage = null;
            }
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
            Mod.Core.SettingsVUI = this;
            EventBus.Subscribe(this);
        }

        public void HandleModDisable()
        {
            EventBus.Unsubscribe(this);
            Detach();
            Mod.Core.SettingsVUI = null;
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
