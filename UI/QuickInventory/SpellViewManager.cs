using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.UnitSettings;
using Kingmaker.Utility;
using ModMaker;
using QuickCast.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static QuickCast.Main;

namespace QuickCast.UI.QuickInventory
{
    public class SpellViewManager : ViewManager, IModEventHandler, ISelectionHandler, IViewChangeHandler
    {
        private DateTime _time;
        public int Priority => 500;

        public static SpellViewManager CreateObject(UnitEntityData unit)
        {
            var scrollview = GameObject.Instantiate(Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate"), Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViews"), false);
            scrollview.name = $"ScrollViewSpells{unit.CharacterName}";
            scrollview.gameObject.SetActive(true);
            var scrollViewMono = scrollview.gameObject.AddComponent<SpellViewManager>();
            scrollViewMono._unit = unit;
            scrollViewMono._viewPortType = MainWindowManager.ViewPortType.Spells;
            return scrollViewMono;
        }

        public override void Start()
        {
            base.Start();
            _time = DateTime.Now;

            BuildHeaders(ref _levelContentTransforms, ref _levelTransforms);
            BuildList();
            RestoreHeaders();
            EventBus.Subscribe(this);
            transform.gameObject.SetActive(false);
        }

        void Update()
        {
            if (DateTime.Now > _time)
            {
                BuildList();
                _time = DateTime.Now + TimeSpan.FromMilliseconds(SetWrap.RefreshRate);
                UpdateUsesAndDC();
            }
        }
        public void BuildList()
        {
            List<MechanicActionBarSlotSpell> abilities = new List<MechanicActionBarSlotSpell>();

            foreach (var book in _unit.Spellbooks)
            {
                if (book.Blueprint.Spontaneous)
                {
                    foreach (var spell in book.GetAllKnownSpells().Where(x => x.GetAvailableForCastCount() > 0 || x.SpellLevel == 0))
                    {
                        abilities.Add(new MechanicActionBarSlotSpontaneousSpell(spell));
                    }
                    for (int i = 1; i <= 10; i++)
                    {
                        foreach (var custom in book.GetCustomSpells(i))
                        {
                            abilities.Add(new MechanicActionBarSlotSpontaneousSpell(custom));
                        }
                    }
                }
                else
                {
                    foreach (var spell in book.GetKnownSpells(0))
                    {
                        abilities.Add(new MechanicActionBarSlotSpontaneousSpell(spell));
                    }

                    foreach (var spell in book.GetAllMemorizedSpells().Where(x => x.Spell.GetAvailableForCastCount() > 0))
                    {
                        abilities.Add(new MechanicActionBarSlotMemorizedSpell(spell));
                    }

                    for (int i = 1; i <= 10; i++)
                    {
                        foreach (var custom in book.GetCustomSpells(i))
                        {
                            if(custom.SpellSlot != null) abilities.Add(new MechanicActionBarSlotMemorizedSpell(custom.SpellSlot));
                        }
                    }
                }
            }

            foreach (var a in abilities)
            {
                if (!Entries.ContainsKey(a.Spell.ToString()))
                {
                    a.Unit = _unit;
                    Entries.Add(a.Spell.ToString(), InsertTransform(a, a.Spell.Name, _levelContentTransforms[a.Spell.SpellLevel], _levelTransforms[a.Spell.SpellLevel]));
                }
            }

            foreach (var v in Entries.ToList().Select(x => x.Key).Except(abilities.Select(x => x.Spell.ToString())))
            {
                var slot = (MechanicActionBarSlotSpell)Entries[v].MSlot;
                RemoveTransform(v, Entries, _levelContentTransforms[slot.Spell.SpellLevel], _levelTransforms[slot.Spell.SpellLevel]);
            }

            SortTransforms();
        }

        public void UpdateUsesAndDC()
        {
            foreach (var kvp in Entries)
            {
                var slot = (MechanicActionBarSlotSpell)kvp.Value.MSlot;
                if (slot?.Spell?.SpellLevel == null)
                    return;

                if (slot.Spell.SpellLevel == 0)
                    kvp.Value.UsesText.text = "-";
                else
                    kvp.Value.UsesText.text = slot.Spell.GetAvailableForCastCount().ToString();

                kvp.Value.DCText.text = slot.Spell.CalculateParams().DC.ToString();
            }
        }

        public void OnUnitSelectionAdd(UnitEntityData selected)
        {
            BuildList();
            UpdateUsesAndDC();

            if (!Game.Instance.UI.SelectionManager.IsSingleSelected)
            {
                _multiSelected.gameObject.SetActive(true);
                _multiSelected.SetAsLastSibling();
                return;
            }
            else if (Game.Instance.UI.SelectionManager.FirstSelectUnit == _unit && Mod.Core.UI.MainWindowManager.CurrentViewPort == MainWindowManager.ViewPortType.Spells)
            {
                bool hasSpells = false;
                foreach (var sb in selected.Spellbooks)
                    hasSpells = sb.GetAllKnownSpells().Any();

                if (!hasSpells)
                {
                    _noSpells.gameObject.SetActive(true);
                    _noSpells.SetAsLastSibling();
                }
                else
                {
                    foreach (RectTransform t in transform.parent)
                        t.gameObject.SetActive(false);
                    transform.gameObject.SetActive(true);
                    transform.SetAsLastSibling();
                }
            }
        }

        public void OnUnitSelectionRemove(UnitEntityData selected)
        {
        }

        public void HandleModEnable()
        {
        }

        public void HandleModDisable() => EventBus.Unsubscribe(this);

        public void HandleViewChange() => this.OnUnitSelectionAdd(Game.Instance.UI.SelectionManager.SelectedUnits.FirstOrDefault<UnitEntityData>());
    }
}
