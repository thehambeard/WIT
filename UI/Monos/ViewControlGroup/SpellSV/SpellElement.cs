using Kingmaker.UnitLogic.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace QuickCast.UI.Monos.ViewControlGroup.SpellSV
{
    internal class SpellElement : Element
    {
        public AbilityData Spell;

        private TextMeshProUGUI _text;

        public void Initialize(AbilityData spell)
        {
            name = $"{spell.Name}-SpellElement";
            Spell = spell;
            _text = GetComponentInChildren<TextMeshProUGUI>();

            SetText(spell.Name);

            gameObject.SetActive(true);
        }

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}
