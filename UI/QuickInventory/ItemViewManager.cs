﻿using Kingmaker;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WIT.Utilities;
using static WIT.UI.QuickInventory.MainWindowManager;

namespace WIT.UI.QuickInventory
{
    public class ItemViewManager : ViewManager
    {
        private UsableItemType _usableType;
        protected override void Awake()
        {
            _unit = _currentUnitProcessing;
            _viewPortType = _currentViewProcessing;

            switch(_viewPortType)
            {
                case ViewPortType.Scrolls:
                    _usableType = UsableItemType.Scroll;
                    break;
                case ViewPortType.Potions:
                    _usableType = UsableItemType.Potion;
                    break;
                case ViewPortType.Wands:
                    _usableType = UsableItemType.Wand;
                    break;
            }

            BuildList();
            base.Awake();
        }

        public static ItemViewManager CreateObject(UnitEntityData unit, ViewPortType viewPortType)
        {
            _currentUnitProcessing = unit;
            _currentViewProcessing = viewPortType;
            var scrollview = GameObject.Instantiate(Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate"), Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViews"), false);
            scrollview.name = $"ScrollViewItem{viewPortType.ToString()}{unit.CharacterName}";
            scrollview.gameObject.SetActive(true);
            return scrollview.gameObject.AddComponent<ItemViewManager>();
        }

        protected override void Update()
        {
            if (_isDirty)
            {
                BuildList();
                //UpdateUsesAndDC();
            }
            base.Update();
        }
        public void BuildList()
        {
            List<AbilityData> abilities = new List<AbilityData>();
            _spells.Clear();

            foreach (var item in _unit.Inventory
                .Where(c => ((c.Blueprint as BlueprintItemEquipmentUsable != null) && c.InventorySlotIndex >= 0 && (c.Blueprint as BlueprintItemEquipmentUsable).Type == _usableType))
                .OrderBy(item => item.Ability.Data.SpellLevel)
                .ThenBy(item => item.Name))
                
            {              
                _spells.Add(item.Ability.Data);
            }
        }

        public void UpdateUsesAndDC()
        {
            foreach (var kvp in _abilityLookup)
            {
                if (kvp.Key.SpellLevel == 0)
                    continue;
                if (kvp.Key.IsSpontaneous && kvp.Key.Spellbook != null)
                    kvp.Value.UsesText.text = kvp.Key.Spellbook.GetSpontaneousSlots(kvp.Key.SpellLevel).ToString();
                else if (kvp.Key.SpellSlot != null)
                    kvp.Value.UsesText.text = kvp.Key.SpellSlot.BusySlotsCount.ToString();
                kvp.Value.DCText.text = kvp.Key.CalculateParams().DC.ToString();
            }
        }
    }
}
