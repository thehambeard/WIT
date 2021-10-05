﻿using Kingmaker;
using Kingmaker.PubSubSystem;
using ModMaker;
using ModMaker.Utility;
using UnityEngine;
using static WIT.Main;
using WIT.UI.QuickInventory;
using System.Collections.Generic;
using Kingmaker.EntitySystem.Entities;

namespace WIT.Controllers
{
    internal class SpellViewController : IModEventHandler, IAreaLoadingStagesHandler
    {
        public int Priority => 400;

        public Dictionary<UnitEntityData, SpellViewManager> SpellViewManage { get; private set; }

        public void Attach()
        {
            SpellViewManage = new Dictionary<UnitEntityData, SpellViewManager>();

            foreach (var unit in Game.Instance.Player.Party)
            {
                SpellViewManage.Add(unit, SpellViewManager.CreateObject(unit));
            }
        }

        public void Detach()
        {
            if (SpellViewManage != null)
            {
                foreach (KeyValuePair<UnitEntityData, SpellViewManager> kvp in SpellViewManage)
                {
                    kvp.Value.SafeDestroy();
                }
                SpellViewManage = null;
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
            Mod.Core.SpellVUI = this;
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