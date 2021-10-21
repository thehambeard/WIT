// Decompiled with JetBrains decompiler
// Type: WIT.UI.QuickInventory.ItemViewManager
// Assembly: WIT, Version=1.0.0.29198, Culture=neutral, PublicKeyToken=null
// MVID: 08962BC7-35F8-4C79-A4D8-CCD608C2370C
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathfinder Second Adventure Debug\Mods\WIT\WIT.dll

using Kingmaker;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.UnitSettings;
using ModMaker;
using ModMaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WIT.Utilities;

namespace WIT.UI.QuickInventory
{
    public class ItemViewManager :
      MonoBehaviour,
      IModEventHandler,
      ISelectionHandler,
      ISubscriber,
      IGlobalSubscriber,
      IViewChangeHandler
    {
        private MainWindowManager.ViewPortType _viewPort;
        private static MainWindowManager.ViewPortType _currentViewProcessing;
        private UsableItemType _itemType;
        public bool _isDirty = true;
        private Dictionary<ItemEntity, ItemEntryData> _items;
        private Transform _template;
        private DateTime _time;
        private Transform _multiSelected;
        private Transform _noSpells;

        public int Priority => 300;

        public static ItemViewManager CreateObject(
          MainWindowManager.ViewPortType viewPortType)
        {
            ItemViewManager._currentViewProcessing = viewPortType;
            Transform transform1 = UnityEngine.Object.Instantiate<Transform>(Game.Instance.UI.Canvas.transform.FirstOrDefault((Func<Transform, bool>)(x => x.name == "ScrollViewTemplate")), Game.Instance.UI.Canvas.transform.FirstOrDefault((Func<Transform, bool>)(x => x.name == "ScrollViews")), false);
            transform1.name = "ScrollViewItems" + viewPortType.ToString();
            transform1.gameObject.SetActive(true);
            Transform original1 = transform1.Find("Viewport/Content/SpellLevel");
            Transform original2 = transform1.Find("Viewport/Content/SpellLevelContent");
            for (int index = 0; index <= 10; ++index)
            {
                Transform transform2 = UnityEngine.Object.Instantiate<Transform>(original1, original1.parent, false);
                Transform transform3 = UnityEngine.Object.Instantiate<Transform>(original2, original2.parent, false);
                transform3.name = string.Format("SpellLevelContent{0}", (object)index);
                transform3.gameObject.SetActive(false);
                transform3.Find("Spell").SafeDestroy();
                transform2.name = string.Format("SpellLevel{0}", (object)index);
                transform2.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("Level {0} Items", (object)index);
                transform2.gameObject.SetActive(false);
            }
            original1.SafeDestroy();
            original2.SafeDestroy();
            return transform1.gameObject.AddComponent<ItemViewManager>();
        }

        private void Awake()
        {
            this._template = Game.Instance.UI.Canvas.transform.FirstOrDefault((Func<Transform, bool>)(x => x.name == "ScrollViewTemplate"));
            this._items = new Dictionary<ItemEntity, ItemEntryData>();
            this._viewPort = ItemViewManager._currentViewProcessing;
            this._multiSelected = this.transform.FindTargetParent("ScrollViews").FirstOrDefault((Func<Transform, bool>)(x => x.name == "MultiSelected"));
            this._noSpells = this.transform.parent.FirstOrDefault((Func<Transform, bool>)(x => x.name == "NoSpells"));
            this._time = DateTime.Now + TimeSpan.FromMilliseconds(0.5);
            switch (this._viewPort)
            {
                case MainWindowManager.ViewPortType.Scrolls:
                    this._itemType = UsableItemType.Scroll;
                    break;
                case MainWindowManager.ViewPortType.Potions:
                    this._itemType = UsableItemType.Potion;
                    break;
                case MainWindowManager.ViewPortType.Wands:
                    this._itemType = UsableItemType.Wand;
                    break;
            }
            this.BuildList();
            this.OnUnitSelectionAdd(Game.Instance.UI.SelectionManager.SelectedUnits.FirstOrDefault<UnitEntityData>());
            EventBus.Subscribe((object)this);
        }

        public void BuildList()
        {
            List<ItemEntity> list = Game.Instance.Player.Inventory.Where<ItemEntity>((Func<ItemEntity, bool>)(x => x.Blueprint is BlueprintItemEquipmentUsable && ((BlueprintItemEquipmentUsable)x.Blueprint).Type == this._itemType)).OrderBy<ItemEntity, string>((Func<ItemEntity, string>)(x => x.Name)).ToList<ItemEntity>();
            foreach (ItemEntity key in list)
            {
                if (!this._items.ContainsKey(key))
                    this._items.Add(key, this.InsertSpellTransform(key));
            }
            foreach (ItemEntity itemEntity in this._items.ToList<KeyValuePair<ItemEntity, ItemEntryData>>().Select<KeyValuePair<ItemEntity, ItemEntryData>, ItemEntity>((Func<KeyValuePair<ItemEntity, ItemEntryData>, ItemEntity>)(x => x.Key)).Except<ItemEntity>((IEnumerable<ItemEntity>)list).Reverse<ItemEntity>())
                this.RemoveSpellTransform(itemEntity);
            this.SortTransforms();
        }

        private void SortTransforms()
        {
            int num = 0;
            foreach (KeyValuePair<ItemEntity, ItemEntryData> keyValuePair in (IEnumerable<KeyValuePair<ItemEntity, ItemEntryData>>)this._items.OrderBy<KeyValuePair<ItemEntity, ItemEntryData>, string>((Func<KeyValuePair<ItemEntity, ItemEntryData>, string>)(x => x.Key.Name)))
                keyValuePair.Value.Transform.SetSiblingIndex(num++);
        }

        private void Update()
        {
            if (!(DateTime.Now > this._time))
                return;
            this.BuildList();
            this._time = DateTime.Now + TimeSpan.FromMilliseconds(0.5);
        }

        private ItemEntryData InsertSpellTransform(ItemEntity item)
        {
            string[] strArray = new string[4]
            {
        "",
        "Scroll of ",
        "Potion of ",
        "Wand of "
            };
            Transform parent = this.transform.Find(string.Format("Viewport/Content/SpellLevelContent{0}", (object)item.GetSpellLevel()));
            Transform transform1 = this.transform.Find(string.Format("Viewport/Content/SpellLevel{0}", (object)item.GetSpellLevel()));
            Transform transform2 = UnityEngine.Object.Instantiate<Transform>(this._template.Find("Viewport/Content/SpellLevelContent/Spell"), parent, false);
            transform2.name = item.Name;
            TextMeshProUGUI component = transform2.Find("SpellText").GetComponent<TextMeshProUGUI>();
            component.text = item.Name.Replace(strArray[(int)this._viewPort], "");
            component.color = new Color(0.31f, 0.31f, 0.31f);
            Button componentInChildren = transform2.GetComponentInChildren<Button>();
            ItemEntryData entry = new ItemEntryData()
            {
                Transform = transform2,
                Button = componentInChildren,
                Data = item,
                DCText = transform2.FirstOrDefault((Func<Transform, bool>)(x => x.name == "DCText")).GetComponent<TextMeshProUGUI>(),
                UsesText = transform2.FirstOrDefault((Func<Transform, bool>)(x => x.name == "UsesText")).GetComponent<TextMeshProUGUI>()
            };
            componentInChildren.onClick.AddListener((UnityAction)(() => this.RunCommand(entry)));
            parent.gameObject.SetActive(true);
            transform1.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parent);
            return entry;
        }

        private void RunCommand(ItemEntryData entry)
        {
            if (Game.Instance.UI.SelectionManager.SelectedUnits.Count != 1)
                return;
            UnitEntityData wielder = Game.Instance.UI.SelectionManager.SelectedUnits.FirstOrDefault<UnitEntityData>();
            MechanicActionBarSlotAbility actionBarSlotAbility = new MechanicActionBarSlotAbility();
            entry.Data.OnDidEquipped(wielder);
            actionBarSlotAbility.Ability = entry.Data.Ability.Data;
            actionBarSlotAbility.Unit = wielder;
            actionBarSlotAbility.OnClick();
            entry.Data.OnWillUnequip();
        }

        private void RemoveSpellTransform(ItemEntity item)
        {
            Transform transform1 = this.transform.Find(string.Format("Viewport/Content/SpellLevelContent{0}", (object)item.GetSpellLevel()));
            Transform transform2 = this.transform.Find(string.Format("Viewport/Content/SpellLevel{0}", (object)item.GetSpellLevel()));
            UnityEngine.Object.DestroyImmediate((UnityEngine.Object)this._items[item].Transform.gameObject);
            this._items.Remove(item);
            if (transform1.childCount <= 0)
            {
                transform1.gameObject.SetActive(false);
                transform2.gameObject.SetActive(false);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform1);
        }

        public void OnUnitSelectionAdd(UnitEntityData selected)
        {
            if (!Game.Instance.UI.SelectionManager.IsSingleSelected)
            {
                this._multiSelected.gameObject.SetActive(true);
                this._multiSelected.SetAsLastSibling();
            }
            else
            {
                if (Main.Mod.Core.UI.MainWindowManager.CurrentViewPort != this._viewPort)
                    return;
                if (this._items.Count <= 0)
                {
                    this._noSpells.gameObject.SetActive(true);
                    this._noSpells.SetAsLastSibling();
                }
                else
                    this.transform.SetAsLastSibling();
            }
        }

        public void OnUnitSelectionRemove(UnitEntityData selected)
        {
        }

        public void HandleModEnable()
        {
        }

        public void HandleModDisable() => EventBus.Unsubscribe((object)this);

        public void HandleViewChange() => this.OnUnitSelectionAdd(Game.Instance.UI.SelectionManager.SelectedUnits.FirstOrDefault<UnitEntityData>());
    }
}
