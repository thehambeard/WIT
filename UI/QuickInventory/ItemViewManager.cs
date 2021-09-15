using Kingmaker;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Items;
using System.Collections.Generic;

namespace WIT.UI.QuickInventory
{
    public class ItemViewManager : ViewManager
    {
        protected ItemsCollection _stash;
        protected UsableItemType _itemType;
        protected List<string> _excludeGUIDs;
        protected List<string> _includeGUIDs;

        public override void Start()
        {
            string itemTypeString;
            switch (_itemType)
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

            _HeaderTitles = new List<string>()
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

            _stash = Game.Instance.Player.Inventory;
            _includeGUIDs = new List<string>();
            _excludeGUIDs = new List<string>();

            foreach (string s in _HeaderTitles)
                _ViewContent.Add(new Dictionary<object, ItemButtonManager>());

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

            //foreach (var v in _ViewContent)
            //{
            //    oldcount += v.Count;
            //}

            //foreach (var item in _stash.Where(c =>
            //    (c.Blueprint as BlueprintItemEquipmentUsable != null &&
            //    (c.Blueprint as BlueprintItemEquipmentUsable)?.Type == _itemType &&
            //    !_excludeGUIDs.Contains(c.Blueprint.AssetGuid)) ||
            //    _includeGUIDs.Contains(c.Blueprint.AssetGuid))
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
            //    foreach (var v in _ViewContent)
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