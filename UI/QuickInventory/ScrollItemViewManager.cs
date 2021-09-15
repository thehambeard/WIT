using Kingmaker.Blueprints.Items.Equipment;

namespace WIT.UI.QuickInventory
{
    internal class ScrollItemViewManager : ItemViewManager
    {
        public override void Start()
        {
            _itemType = UsableItemType.Scroll;
            base.Start();
        }

        //public override void Update()
        //{
        //    base.Update();
        //}
    }
}