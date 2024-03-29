﻿using Kingmaker;
using Kingmaker.PubSubSystem;
using ModMaker;
using ModMaker.Utility;
using QuickCast.UI.QuickInventory;
using System.Collections.Generic;
using UnityEngine;
using static QuickCast.Main;
using static QuickCast.UI.QuickInventory.MainWindowManager;

namespace QuickCast.Controllers
{
    internal class ItemViewController : IModEventHandler, IAreaLoadingStagesHandler, IAreaHandler
    {
        public int Priority => 400;

        public List<ItemViewManager> ItemViewManage { get; private set; }

        public void Attach()
        {
            ItemViewManage = new List<ItemViewManager>()
            {

                    ItemViewManager.CreateObject(ViewPortType.Scrolls),
                    ItemViewManager.CreateObject(ViewPortType.Potions),
                    ItemViewManager.CreateObject(ViewPortType.Wands)
            };
        }

        public void Detach()
        {
            if (ItemViewManage != null)
            {
                foreach (var kvp in ItemViewManage)
                {
                    EventBus.Unsubscribe(kvp);
                    kvp.SafeDestroy();
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
            Mod.Core.ItemVUI = null;
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