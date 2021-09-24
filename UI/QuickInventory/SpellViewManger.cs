using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace WIT.UI.QuickInventory
{
    class SpellViewManager : MonoBehaviour
    {
        static int _index;
        static UnitEntityData _unit;

        public static SpellViewManager CreateObject(UnitEntityData unit, int index)
        {
            _index = index;
            _unit = unit;

            return null;
        }

        void Awake()
        {
        }
        void Update()
        {

        }
    }
}
