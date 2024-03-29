﻿using Kingmaker;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.UnitSettings;
using Kingmaker.Utility;
using ModMaker;
using QuickCast.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuickCast.UI.QuickInventory
{
    public class ItemViewManager : ViewManager, IModEventHandler, ISelectionHandler, IViewChangeHandler
    {
        private UsableItemType _itemType;
        private DateTime _time;

        public int Priority => 300;

        public static ItemViewManager CreateObject(MainWindowManager.ViewPortType viewPortType)
        {
            var scrollview = GameObject.Instantiate(Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate"), Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViews"), false);
            scrollview.name = "ScrollViewItems" + viewPortType.ToString();
            scrollview.gameObject.SetActive(true);
            var scrollViewMono = scrollview.gameObject.AddComponent<ItemViewManager>();
            scrollViewMono._viewPortType = viewPortType;
            return scrollViewMono;
        }

        public override void Start()
        {
            base.Start();

            _time = DateTime.Now;

            BuildHeaders(ref _levelContentTransforms, ref _levelTransforms);

            switch (_viewPortType)
            {
                case MainWindowManager.ViewPortType.Scrolls:
                    _itemType = UsableItemType.Scroll;
                    break;
                case MainWindowManager.ViewPortType.Potions:
                    _itemType = UsableItemType.Potion;
                    break;
                case MainWindowManager.ViewPortType.Wands:
                    _itemType = UsableItemType.Wand;
                    break;
            }

            BuildList();
            RestoreHeaders();
            EventBus.Subscribe(this);
            transform.gameObject.SetActive(false);
        }

        public void BuildList()
        {
            string[] filter = new string[]
            {
                "",
                "Scroll of ",
                "Potion of ",
                "Wand of "
            };

            List<MechanicActionBarSlotItem> list = new List<MechanicActionBarSlotItem>();
            foreach (var item in Game.Instance.Player.Inventory)
            {
                if ((item.Blueprint is BlueprintItemEquipmentUsable && ((BlueprintItemEquipmentUsable)item.Blueprint).Type == _itemType) || (_itemType == UsableItemType.Potion && (item.Blueprint.AssetGuid == "4639724c4a9cc9544a2f622b66931658" || item.Blueprint.AssetGuid == "fd56596e273d1ff49a8c29cc9802ae6e" || item.Blueprint.AssetGuid == "a8bc157a846e2d64498915cadd026aef")))
                {
                    list.Add(new MechanicActionBarSlotItem() { Item = (ItemEntityUsable)item });
                }
            }
            foreach (MechanicActionBarSlotItem item in list)
            {
                if (!Entries.ContainsKey(item.Item.ToString()))
                    Entries.Add(item.Item.ToString(), InsertTransform(item, item.Item.Name.Replace(filter[(int)_viewPortType], ""), _levelContentTransforms[item.Item.GetSpellLevel()], _levelTransforms[item.Item.GetSpellLevel()]));
            }
            foreach (var v in Entries.ToList().Select(x => x.Key).Except(list.Select(x => x.Item.ToString())))
            {
                var slot = (MechanicActionBarSlotItem)Entries[v].MSlot;
                RemoveTransform(v, Entries, _levelContentTransforms[slot.Item.GetSpellLevel()], _levelTransforms[slot.Item.GetSpellLevel()]);
            }

            SortTransforms();
        }

        private void Update()
        {
            if (!(DateTime.Now > _time))
                return;
            BuildList();
            UpdateUsesAndDC();
            _time = DateTime.Now + TimeSpan.FromMilliseconds(SetWrap.RefreshRate);
        }

        public void UpdateUsesAndDC()
        {
            foreach (var kvp in Entries)
            {
                var slot = (MechanicActionBarSlotItem)kvp.Value.MSlot;
                kvp.Value.UsesText.text = slot.Item.Charges.ToString();
                kvp.Value.DCText.text = slot.Item.Count.ToString();
            }
        }

        public void OnUnitSelectionAdd(UnitEntityData selected)
        {
            if (Game.Instance.UI.SelectionManager.SelectedUnits.Count() != 1)
            {
                _multiSelected.gameObject.SetActive(true);
                _multiSelected.SetAsLastSibling();
                return;
            }
            else if (Main.Mod.Core.UI.MainWindowManager.CurrentViewPort == _viewPortType)
            {
                if (Entries.Count <= 0)
                {
                    _noSpells.gameObject.SetActive(true);
                    _noSpells.SetAsLastSibling();
                }
                else
                {
                    BuildList();
                    UpdateUsesAndDC();
                    gameObject.SetActive(true);
                    transform.SetAsLastSibling();
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void OnUnitSelectionRemove(UnitEntityData selected)
        {
        }

        public void HandleModEnable()
        {
        }

        public void HandleModDisable() => EventBus.Unsubscribe(this);

        public void HandleViewChange() => this.OnUnitSelectionAdd(Game.Instance.UI.SelectionManager.SelectedUnits.FirstOrDefault<UnitEntityData>());
    }
}
