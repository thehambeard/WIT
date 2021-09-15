using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WIT.UI.QuickInventory
{
    public class SpellViewManager : ViewManager, ISelectionManagerUIHandler
    {
        private UnitEntityData _character;
        private CanvasGroup _canvas;
        private bool _isDirty;

        public void HandleSwitchSelectionUnitInGroup()
        {
            _isDirty = true;
        }

        public override void Start()
        {
            EventBus.Subscribe(this);

            _character = Game.Instance.Player.MainCharacter.Value;
            _canvas = gameObject.GetComponent<CanvasGroup>();

            _HeaderTitles = new List<string>()
            {
                "Level 0 Spells",
                "Level 1 Spells",
                "Level 2 Spells",
                "Level 3 Spells",
                "Level 4 Spells",
                "Level 5 Spells",
                "Level 6 Spells",
                "Level 7 Spells",
                "Level 8 Spells",
                "Level 9 Spells",
                "No Spells",
                "Select Character"
            };

            _ViewContent.AddRange(_HeaderTitles.Select(s => new Dictionary<object, ItemButtonManager>()));

            base.Start();
            _isDirty = true;
        }

        public override void Update()
        {
            if (_isDirty) UpdateSpells();
            base.Update();
        }

        private void UpdateSpells()
        {
            _isDirty = false;
            List<ItemButtonManager> newSpells = new List<ItemButtonManager>();
            int oldcount = 0;
            int newcount = 0;

            var selection = SelectionManager.Instance.SelectedUnits;
            if ((selection != null) && (selection.Count == 1) && (selection[0] == _character))
            {
                foreach (var v in _ViewContent)
                {
                    oldcount += v.Count;
                }

                foreach (var book in _character.Descriptor.Spellbooks)
                {
                    foreach (var spell in book.GetAllKnownSpells())
                    {
                        if (spell.SpellLevel >= 0 && spell.SpellLevel < 10)
                            newSpells.Add(Ensure(spell, spell.SpellLevel, newcount++, ref _isDirty));
                    }
                }

                if (newcount != oldcount)
                {
                    _isDirty = true;
                }
            }
            else if ((selection != null) && (selection.Count == 1))
            {
                _character = selection[0];
                _isDirty = true;
            }
            else
            {
                _isDirty = true;
            }

            if (_isDirty)
            {
                foreach (var v in _ViewContent)
                {
                    foreach (ItemButtonManager button in v.Values.Except(newSpells).ToList())
                    {
                        RemoveButton(button);
                    }
                }
            }
        }
    }
}