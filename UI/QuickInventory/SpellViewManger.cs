using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Utility.UnitDescription;
using UnityEngine;
using UnityEngine.UI;

namespace WIT.UI.QuickInventory
{
    class SpellViewManager : MonoBehaviour
    {
        static int _index;
        static UnitEntityData _unit;
        static RectTransform _template;
        
        public static SpellViewManager CreateObject(UnitEntityData unit)
        {
            _unit = unit;
            _template = (RectTransform) Game.Instance.UI.Canvas.transform.Find("QuickInventory/QuickWindow/ScrollViews/ScrollViewSpells");

            return _template == null ? null : GameObject.Instantiate(_template, _template.parent, false).gameObject.AddComponent<SpellViewManager>();
        }

        void Awake()
        {
            foreach (var spell in _unit.Spellbooks)
            {

            }
                
        }
        void Update()
        {

        }
    }
}
