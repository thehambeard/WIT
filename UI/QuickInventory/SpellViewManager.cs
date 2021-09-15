using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using static WIT.Main;
using Kingmaker;
using WIT.Utilities;
using Kingmaker.UI;
using TMPro;
using Kingmaker.UI.Common;
using UnityEngine.UI;
using Kingmaker.UI.Constructor;
using UnityEngine.Events;
using Kingmaker.Localization;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic;
using Kingmaker.UI.Selection;
using DG.Tweening;
using System.Collections;
using JetBrains.Annotations;
using Kingmaker.View;

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

            m_HeaderTitles = new List<string>()
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

            foreach (string s in m_HeaderTitles)
                m_ViewContent.Add(new Dictionary<object, ItemButtonManager>());

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
                foreach (var v in m_ViewContent)
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
                foreach (var v in m_ViewContent)
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

