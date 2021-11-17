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
    internal class SpecialViewController : IModEventHandler, IAreaLoadingStagesHandler, IPartyHandler
    {
        public int Priority => 400;

        public Dictionary<UnitEntityData, SpecialViewManager> SpecialViewManagers { get; private set; }

        public void Attach()
        {
            SpecialViewManagers = new Dictionary<UnitEntityData, SpecialViewManager>();

            foreach (var unit in Game.Instance.Player.Party)
            {
                SpecialViewManagers.Add(unit, SpecialViewManager.CreateObject(unit));
                foreach (var pet in unit.Pets)
                {
                    SpecialViewManagers.Add(pet.Entity, SpecialViewManager.CreateObject(pet.Entity));
                }
            }
        }

        public void Detach()
        {
            if (SpecialViewManagers != null)
            {
                foreach (KeyValuePair<UnitEntityData, SpecialViewManager> kvp in SpecialViewManagers)
                {
                    kvp.Value.SafeDestroy();
                }
                SpecialViewManagers = null;
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
            Mod.Core.SpecialVUI = this;
            EventBus.Subscribe(this);
        }

        public void HandleModDisable()
        {
            EventBus.Unsubscribe(this);
            Detach();
            Mod.Core.SpecialVUI = null;
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
            PartyChanged();
        }
    }
}