using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using ModMaker;
using ModMaker.Utility;
using QuickCast.UI.QuickInventory;
using System.Collections.Generic;
using UnityEngine;
using static QuickCast.Main;

namespace QuickCast.Controllers
{
    internal class SpellViewController : IModEventHandler, IAreaLoadingStagesHandler, IPartyHandler
    {
        public int Priority => 400;

        public Dictionary<UnitEntityData, SpellViewManager> SpellViewManage { get; private set; }

        public void Attach()
        {
            SpellViewManage = new Dictionary<UnitEntityData, SpellViewManager>();

            foreach (var unit in Game.Instance.Player.Party)
            {
                SpellViewManage.Add(unit, SpellViewManager.CreateObject(unit));
                foreach (var pet in unit.Pets)
                {
                    SpellViewManage.Add(pet.Entity, SpellViewManager.CreateObject(pet.Entity));
                }
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

        private void PartyChanged()
        {
            Update();
        }

        public void HandleAddCompanion(UnitEntityData unit)
        {
            PartyChanged();
        }

        public void HandleCompanionActivated(UnitEntityData unit)
        {
            PartyChanged();
        }

        public void HandleCompanionRemoved(UnitEntityData unit, bool stayInGame)
        {
            PartyChanged();
        }

        public void HandleCapitalModeChanged()
        {
            Update();
        }
    }
}