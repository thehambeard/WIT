using Kingmaker.Blueprints.Items.Equipment;

namespace WIT.UI.QuickInventory
{
    internal class PotionItemViewManager : ItemViewManager
    {
        public override void Start()
        {
            _itemType = UsableItemType.Potion;
            base.Start();
        }

        //public override void Update()
        //{
        //        base.Update();
        //}
    }
}