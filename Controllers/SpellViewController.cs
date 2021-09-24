using Kingmaker;
using Kingmaker.PubSubSystem;
using ModMaker;
using ModMaker.Utility;
using UnityEngine;
using static WIT.Main;
using WIT.UI.QuickInventory;
using System.Collections.Generic;

namespace WIT.Controllers
{
    internal class SpellViewController : IModEventHandler, IAreaLoadingStagesHandler
    {
        public int Priority => 400;

        public List<SpellViewManager> SpellViewManage { get; private set; }

        public void Attach()
        {
            int i = 0;
            if (SpellViewManage == null)
            {
                foreach (var unit in Game.Instance.Player.Party)
                {
                    SpellViewManage = SpellViewManager.CreateObject(unit, i++);
                }
            }
        }

        public void Detach()
        {
            foreach (var unit in SpellViewManage)
            {
                unit.SafeDestroy();
            }
            SpellViewManage = null;
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