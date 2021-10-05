using Kingmaker;
using Kingmaker.PubSubSystem;
using ModMaker;
using ModMaker.Utility;
using UnityEngine;
using static WIT.Main;
using WIT.UI.QuickInventory;
using System.Collections.Generic;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Blueprints.Items.Equipment;
using static WIT.UI.QuickInventory.MainWindowManager;

namespace WIT.Controllers
{
    internal class ItemViewController : IModEventHandler, IAreaLoadingStagesHandler
    {
        public int Priority => 400;

        public Dictionary<UnitEntityData, List<ItemViewManager>> ItemViewManage { get; private set; }

        public void Attach()
        {
            ItemViewManage = new Dictionary<UnitEntityData, List<ItemViewManager>>();

            foreach (var unit in Game.Instance.Player.Party)
            {
                ItemViewManage.Add(unit, new List<ItemViewManager>()
                {
                    ItemViewManager.CreateObject(unit, ViewPortType.Scrolls),
                    ItemViewManager.CreateObject(unit, ViewPortType.Potions),
                    ItemViewManager.CreateObject(unit, ViewPortType.Wands)
                });
            }
        }

        public void Detach()
        {
            if (ItemViewManage != null)
            {
                foreach (KeyValuePair<UnitEntityData, List<ItemViewManager>> kvp in ItemViewManage)
                {
                    foreach(var ivm in kvp.Value)
                    ivm.SafeDestroy();
                }
                ItemViewManage = null;
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
            Mod.Core.ItemVUI = this;
            EventBus.Subscribe(this);
        }

        public void HandleModDisable()
        {
            EventBus.Unsubscribe(this);
            Detach();
            Mod.Core.SpellVUI = null;
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