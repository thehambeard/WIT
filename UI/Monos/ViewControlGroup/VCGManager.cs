using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using QuickCast.UI.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Kingmaker.PubSubSystem;
using QuickCast.UI.Monos.ViewControlGroup.SpellSV;

namespace QuickCast.UI.Monos.ViewControlGroup
{
    internal class VCGManager : MonoBehaviour
    {
        public Dictionary<UnitEntityData, SpellSVManager> SpellSVs;

        public void Initialize()
        {
            var contentWrapper = transform.Find("ContentWrapper");

            SpellSVs = new();

            foreach (var unit in Game.Instance.Player.PartyAndPets)
            {
                InitializeScrollView(contentWrapper, unit);
            }
        }

        private void InitializeScrollView(Transform parent, UnitEntityData unit)
        {
            if (SpellSVs == null)
                SpellSVs = new();

            if (!SpellSVs.ContainsKey(unit))
            {
                var ssv = Builders.BuildUI.BuildSpellScrollView(parent, unit);
                ssv.Initialize();
                SpellSVs.Add(unit, ssv);
            }
        }
    }
}
