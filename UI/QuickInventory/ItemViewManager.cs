using Kingmaker;
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
using Kingmaker.Blueprints;
using Kingmaker.Items;
using static WIT.Main;
using Kingmaker.UnitLogic;

namespace WIT.UI.QuickInventory
{
    public class ItemViewManager
    {
        //    private UsableItemType _usableType;
        //    protected override void Awake()
        //    {
        //        _viewPortType = _currentViewProcessing;

        //        switch(_viewPortType)
        //        {
        //            case ViewPortType.Scrolls:
        //                _usableType = UsableItemType.Scroll;
        //                break;
        //            case ViewPortType.Potions:
        //                _usableType = UsableItemType.Potion;
        //                break;
        //            case ViewPortType.Wands:
        //                _usableType = UsableItemType.Wand;
        //                break;
        //        }

        //        BuildList();
        //        base.Awake();
        //    }

        //    public static ItemViewManager CreateObject(ViewPortType viewPortType)
        //    {
        //        _currentViewProcessing = viewPortType;
        //        var scrollview = GameObject.Instantiate(Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate"), Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViews"), false);
        //        scrollview.name = $"ScrollViewItem{viewPortType.ToString()}";
        //        scrollview.gameObject.SetActive(true);
        //        return scrollview.gameObject.AddComponent<ItemViewManager>();
        //    }

        //    protected override void Update()
        //    {
        //        if (IsDirty)
        //        {
        //            BuildList();
        //            //UpdateUsesAndDC();
        //        }
        //        base.Update();
        //    }
        //    public void BuildList()
        //    {
        //        _spells.Clear();

        //        foreach (var item in Game.Instance.Player.Inventory.Items)
        //        {
        //            if (item is ItemEntityUsable)
        //            {
        //                if (((ItemEntityUsable)item).Blueprint.Type == _usableType)
        //                {
        //                    BlueprintItemEquipment blueprint = (BlueprintItemEquipment)item.Blueprint;
        //                    item.Ability = (Ability)_unit.Descriptor.AddFact(blueprint.Ability, null, null);
        //                    if( item.Ability != null) _spells.Add(item.Ability.Data);
        //                    _unit.Descriptor.RemoveFact(blueprint.Ability);
        //                    item.Ability = null;

        //                }
        //            }
        //        }
        //    }

        //    public void UpdateUsesAndDC()
        //    {
        //        //foreach (var kvp in _abilityLookup)
        //        //{
        //        //    if (kvp.Key.SpellLevel == 0)
        //        //        continue;
        //        //    if (kvp.Key.IsSpontaneous && kvp.Key.Spellbook != null)
        //        //        kvp.Value.UsesText.text = kvp.Key.Spellbook.GetSpontaneousSlots(kvp.Key.SpellLevel).ToString();
        //        //    else if (kvp.Key.SpellSlot != null)
        //        //        kvp.Value.UsesText.text = kvp.Key.SpellSlot.BusySlotsCount.ToString();
        //        //    kvp.Value.DCText.text = kvp.Key.CalculateParams().DC.ToString();
        //        //}
        //    }
        //}
    }
}
