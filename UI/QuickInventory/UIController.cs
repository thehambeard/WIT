using Kingmaker;
using Kingmaker.PubSubSystem;
using ModMaker;
using ModMaker.Utility;
using UnityEngine;
using static WIT.Main;

namespace WIT.UI.QuickInventory
{
    internal class UIController : IModEventHandler, IAreaHandler
    {
        public int Priority => 400;

        public UIManager ContainersUI { get; private set; }

        public void Attach()
        {
            if (ContainersUI == null)
                ContainersUI = UIManager.CreateObject();
        }

        public void Detach()
        {
            ContainersUI.SafeDestroy();
            ContainersUI = null;
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
            Attach();

            EventBus.Subscribe(this);
        }

        public void HandleModDisable()
        {
            EventBus.Unsubscribe(this);
            Detach();
            Mod.Core.UI = null;
        }

        public void OnAreaBeginUnloading()
        {
        }

        public void OnAreaDidLoad()
        {
            Attach();
        }
    }
}