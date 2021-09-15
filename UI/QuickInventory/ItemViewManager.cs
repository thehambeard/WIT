using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using static WIT.Main;
using Kingmaker;
using WIT.Utilities;
using Kingmaker.UI;
using TMPro;
using Kingmaker.UI.Common;
using UnityEngine.UI;
using Kingmaker.UI.Constructor;
using UnityEngine.Events;
using Kingmaker.Localization;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic;
using Kingmaker.Items;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.UI.Group;

namespace WIT.UI.QuickInventory
{
    public class ItemViewManager : ViewManager
    {
        protected ItemsCollection m_stash;
        protected UsableItemType m_itemType;
        protected List<string> m_excludeGUIDs;
        protected List<string> m_includeGUIDs;

        

        public override void Start()
        {
            string itemTypeString;
            switch(m_itemType)
            {
                case UsableItemType.Scroll:
                    itemTypeString = "Scrolls";
                    break;
                case UsableItemType.Wand:
                    itemTypeString = "Wands";
                    break;
                case UsableItemType.Potion:
                    itemTypeString = "Potions";
                    break;
                default:
                    itemTypeString = "Errors";
                    break;
            }

            m_HeaderTitles = new List<string>()
            {
                $"Level 0 {itemTypeString}",
                $"Level 1 {itemTypeString}",
                $"Level 2 {itemTypeString}",
                $"Level 3 {itemTypeString}",
                $"Level 4 {itemTypeString}",
                $"Level 5 {itemTypeString}",
                $"Level 6 {itemTypeString}",
                $"Level 7 {itemTypeString}",
                $"Level 8 {itemTypeString}",
                $"Level 9 {itemTypeString}",
                $"No {itemTypeString}",
                "Select Character"
            };

            m_stash = Game.Instance.Player.Inventory;
            m_includeGUIDs = new List<string>();
            m_excludeGUIDs = new List<string>();

            foreach (string s in m_HeaderTitles)
                m_ViewContent.Add(new Dictionary<object, ItemButtonManager>());

            base.Start();
        }

        //public override void Update()
        //{
        //    UpdateItems();
        //    base.Update();
        //}

        private void UpdateItems()
        {
            //bool isDirty = false;
            //List<ItemButtonManager> newItems = new List<ItemButtonManager>();
            //int oldcount = 0;
            //int newcount = 0;

            //foreach (var v in m_ViewContent)
            //{
            //    oldcount += v.Count;
            //}

            //foreach (var item in m_stash.Where(c =>
            //    (c.Blueprint as BlueprintItemEquipmentUsable != null &&
            //    (c.Blueprint as BlueprintItemEquipmentUsable)?.Type == m_itemType &&
            //    !m_excludeGUIDs.Contains(c.Blueprint.AssetGuid)) ||
            //    m_includeGUIDs.Contains(c.Blueprint.AssetGuid))
            //    .OrderBy(item => item.Name))
            //{
            //    newItems.Add(Ensure(item, item.Ability.Data.SpellLevel, newcount++, ref isDirty));
            //}

            //if (newcount != oldcount)
            //{
            //    isDirty = true;
            //}

            //if (isDirty)
            //{
            //    foreach (var v in m_ViewContent)
            //    {
            //        foreach (ItemButtonManager button in v.Values.Except(newItems).ToList())
            //        {
            //            RemoveButton(button);
            //        }
            //    }
            //}
        }
    }
}
