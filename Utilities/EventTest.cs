using JetBrains.Annotations;
using Kingmaker;
using Kingmaker.Armies;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.View;
using ModMaker;
using System;
using System.Reflection;
using static WIT.Main;

namespace WIT.Utilities
{
    //public class EventTest : IModEventHandler, ILearnSpellHandler, IAbilityExecutionProcessHandler, IPartyChangedUIHandler, IItemsCollectionHandler, ISelectionManagerUIHandler, ISelectionHandler
    //{
    //    public int Priority => 200;

    //    public void HandleExecutionProcessEnd(AbilityExecutionContext context)
    //    {
    //        //Not needed redundent
    //        Mod.Debug(MethodBase.GetCurrentMethod());
    //    }

    //    public void HandleExecutionProcessStart(AbilityExecutionContext context)
    //    {
    //        //This fires every time an ability is used. This can trigger the list is dirty.
    //        Mod.Debug(MethodBase.GetCurrentMethod());
    //    }

    //    public void HandleHighlightChange([NotNull] UnitEntityView unit)
    //    {
    //        Mod.Debug(MethodBase.GetCurrentMethod());
    //    }

    //    public void HandleItemsAdded(ItemsCollection collection, ItemEntity item, int count)
    //    {
    //        Mod.Debug(MethodBase.GetCurrentMethod());
    //    }

    //    public void HandleItemsRemoved(ItemsCollection collection, ItemEntity item, int count)
    //    {
    //        Mod.Debug(MethodBase.GetCurrentMethod());
    //    }

    //    public void HandleLearnSpell()
    //    {
    //        Mod.Debug(MethodBase.GetCurrentMethod());
    //    }

    //    public void HandleModDisable()
    //    {
    //        Mod.Debug(MethodBase.GetCurrentMethod());
    //        EventBus.Unsubscribe(this);
    //    }

    //    public void HandleModEnable()
    //    {
    //        Mod.Debug(MethodBase.GetCurrentMethod());
    //        EventBus.Subscribe(this);
    //    }

    //    public void HandlePartyChanged()
    //    {
    //        Mod.Debug(MethodBase.GetCurrentMethod());
    //    }

    //    public void HandleSwitchSelectionUnitInGroup()
    //    {
    //        Mod.Debug(MethodBase.GetCurrentMethod());
    //    }

    //    public void OnUnitSelectionAdd(UnitEntityData selected)
    //    {
    //        Mod.Debug(MethodBase.GetCurrentMethod());
    //    }

    //    public void OnUnitSelectionRemove(UnitEntityData selected)
    //    {
    //        Mod.Debug(MethodBase.GetCurrentMethod());
    //    }
    //}
}