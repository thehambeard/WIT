﻿using Kingmaker;
using Kingmaker.PubSubSystem;
using ModMaker;
using ModMaker.Utility;
using UnityEngine;
using static QuickCast.Main;
using QuickCast.UI.QuickInventory;
using System.Collections.Generic;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Blueprints.Items.Equipment;
using System.Linq;
using static QuickCast.UI.QuickInventory.MainWindowManager;

namespace QuickCast.Controllers
{
    internal class FavoriteViewController : IModEventHandler, IAreaLoadingStagesHandler
    {
        public int Priority => 400;

        public Dictionary<UnitEntityData, FavoriteViewManager> FavoriteViewManagers { get; private set; }

        public void Attach()
        {
            FavoriteViewManagers = new Dictionary<UnitEntityData, FavoriteViewManager>();

            foreach (var unit in Game.Instance.Player.Party)
            {
                FavoriteViewManagers.Add(unit, FavoriteViewManager.CreateObject(unit));
                foreach (var pet in unit.Pets)
                {
                    FavoriteViewManagers.Add(pet.Entity, FavoriteViewManager.CreateObject(pet.Entity));
                }
            }
        }

        public void Detach()
        {
            if (FavoriteViewManagers != null)
            {
                foreach (KeyValuePair<UnitEntityData, FavoriteViewManager> kvp in FavoriteViewManagers)
                {
                    kvp.Value.SafeDestroy();
                }
                FavoriteViewManagers = null;
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
            Mod.Core.FavoriteVUI = this;
            EventBus.Subscribe(this);
        }

        public void HandleModDisable()
        {
            EventBus.Unsubscribe(this);
            Detach();
            Mod.Core.FavoriteVUI = null;
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
                if (!FavoriteViewManagers.ContainsKey(unit))
                {
                    FavoriteViewManagers.Add(unit, FavoriteViewManager.CreateObject(unit));
                }
            }

            foreach (var v in FavoriteViewManagers.ToList().Select(x => x.Key).Except(Game.Instance.Player.Party))
                FavoriteViewManagers.Remove(v);

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