using Kingmaker.Blueprints.Items.Equipment;

namespace WIT.UI.QuickInventory
{
    internal class WandItemViewManager : ItemViewManager
    {
        public override void Start()
        {
            _itemType = UsableItemType.Wand;
            base.Start();
        }

        //public override void Update()
        //{
        //        base.Update();

        //}
    }
}