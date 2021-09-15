using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static WIT.Main;
using Kingmaker;
using Kingmaker.PubSubSystem;
using Kingmaker.Items;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.UI.Vendor;
using Kingmaker.Blueprints.Items.Equipment;
using static WIT.Utilities.SettingsWrapper;

namespace WIT.UI.QuickInventory
{
    class PotionItemViewManager : ItemViewManager
    {
        public override void Start()
        {
            
            m_itemType = UsableItemType.Potion;
            base.Start();
        }

        //public override void Update()
        //{
            
        //        base.Update();
        //}
    }
}
