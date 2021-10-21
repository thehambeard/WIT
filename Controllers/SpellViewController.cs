using Kingmaker;
using Kingmaker.PubSubSystem;
using ModMaker;
using ModMaker.Utility;
using UnityEngine;
using static WIT.Main;
using WIT.UI.QuickInventory;
using System.Collections.Generic;
using Kingmaker.EntitySystem.Entities;
using System.Linq;

namespace WIT.Controllers
{
    internal class SpellViewController : IModEventHandler, IAreaLoadingStagesHandler, IPartyHandler
    {
        public int Priority => 200;

        public Dictionary<UnitEntityData, SpellViewManager> SpellViewManage { get; private set; }

        public void Attach()
        {
            SpellViewManage = new Dictionary<UnitEntityData, SpellViewManager>();

            foreach (var unit in Game.Instance.Player.Party)
            {
                SpellViewManage.Add(unit, SpellViewManager.CreateObject(unit));
                foreach(var pet in unit.Pets)
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
            foreach (var unit in Game.Instance.Player.Party)
            {
                if (!SpellViewManage.ContainsKey(unit))
                {
                    SpellViewManage.Add(unit, SpellViewManager.CreateObject(unit));
                }
            }

            foreach (var v in SpellViewManage.ToList().Select(x => x.Key).Except(Game.Instance.Player.Party))
                SpellViewManage.Remove(v);

            Game.Instance.UI.SelectionManager.SelectAll();
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
        }
    }
}