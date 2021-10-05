using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WIT.Utilities;

namespace WIT.UI.QuickInventory
{
    public class SpellViewManager : ViewManager
    {
        protected override void Awake()
        {
            _unit = _currentUnitProcessing;
            BuildList();
            base.Awake();
        }

        public static SpellViewManager CreateObject(UnitEntityData unit)
        {
            _currentUnitProcessing = unit;
            var scrollview = GameObject.Instantiate(Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate"), Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViews"), false);
            scrollview.name = $"ScrollViewSpells{unit.CharacterName}";
            scrollview.gameObject.SetActive(true);
            return scrollview.gameObject.AddComponent<SpellViewManager>();
        }

        protected override void Update()
        {
            if (_isDirty)
            {
                BuildList();
                UpdateUsesAndDC();
            }
            base.Update();
        }
        public void BuildList()
        {
            List<AbilityData> abilities = new List<AbilityData>();
            _spells.Clear();

            foreach (var book in _unit.Spellbooks)
            {

                if (book.Blueprint.Spontaneous)
                    abilities.AddRange(book.GetAllKnownSpells().ToList());
                else
                {
                    abilities.AddRange(book.GetKnownSpells(0).ToList());
                    abilities.AddRange(book.GetAllMemorizedSpells().Select(x => x.Spell).ToList());
                }
            }
            foreach (var ability in abilities)
            {
                _spells.Add(ability);
            }
        }

        public void UpdateUsesAndDC()
        {
            foreach (var kvp in _abilityLookup)
            {
                if (kvp.Key.SpellLevel == 0)
                    continue;
                if (kvp.Key.IsSpontaneous && kvp.Key.Spellbook != null)
                    kvp.Value.UsesText.text = kvp.Key.Spellbook.GetSpontaneousSlots(kvp.Key.SpellLevel).ToString();
                else if (kvp.Key.SpellSlot != null)
                    kvp.Value.UsesText.text = kvp.Key.SpellSlot.BusySlotsCount.ToString();
                kvp.Value.DCText.text = kvp.Key.CalculateParams().DC.ToString();
            }
        }
    }
}
