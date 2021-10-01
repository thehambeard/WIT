using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WIT.Utilities;

namespace WIT.UI.QuickInventory
{
    class ItemViewManager
    {
        private static UnitEntityData _currentUnitProcessing = null;
        public static SpellViewManager CreateObject(UnitEntityData unit)
        {
            _currentUnitProcessing = unit;
            var scrollview = GameObject.Instantiate(Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate"), Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViews"), false);
            scrollview.name = $"ScrollViewItem{unit.CharacterName}";
            scrollview.gameObject.SetActive(true);
            return scrollview.gameObject.AddComponent<SpellViewManager>();
        }
    }
}
