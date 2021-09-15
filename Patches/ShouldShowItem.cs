 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Items;
using Kingmaker.UI.Common;
using Kingmaker.UI.ServiceWindow;
using static WIT.Utilities.SettingsWrapper;

namespace WIT.Patches
{
    //[HarmonyLib.HarmonyPatch(typeof(ItemsFilter), "ShouldShowItem")]
    //static class ItemsFilter_ShouldShowItem_Patch
    //{
    //    static void Postfix(ref bool __result, ItemEntity item, ItemsFilter.FilterType filter)
    //    {
    //        if (item.Blueprint is BlueprintItemEquipmentUsable && (ItemsFilter.FilterType.Usable ==  filter || ItemsFilter.FilterType.NoFilter == filter))
    //        {
    //            __result = (item.Blueprint as BlueprintItemEquipmentUsable).Type == UsableItemType.Wand && ContainWands ||
    //            (item.Blueprint as BlueprintItemEquipmentUsable).Type == UsableItemType.Scroll && ContainScrolls ||
    //            (item.Blueprint as BlueprintItemEquipmentUsable).Type == UsableItemType.Potion && ContainPotions;
    //        }
    //    }
    //}
}
