using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using ModMaker;
using ModMaker.Utility;
using UnityEngine;
using QuickCast.Utilities;
using static QuickCast.Main;
using UnityEngine.UI;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Commands;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Controllers.Clicks;
using Kingmaker.Utility;
using Kingmaker.UI.UnitSettings;
using DG.Tweening;

namespace QuickCast.UI.QuickInventory
{
    public class SpellViewManager : ViewManager, IModEventHandler, ISelectionHandler, IViewChangeHandler
    {
        private UnitEntityData _unit;
        private static UnitEntityData _currentUnitProcessing;
        public bool _isDirty = true;
        
        private DateTime _time;
        private Transform _multiSelected;
        private Transform _noSpells;
        

        private List<Transform> _levelTransforms;
        private List<Transform> _levelContentTransforms;

        public int Priority => 500;

        public static SpellViewManager CreateObject(UnitEntityData unit)
        {
            _currentUnitProcessing = unit;
            var scrollview = GameObject.Instantiate(Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate"), Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViews"), false);
            scrollview.name = $"ScrollViewSpells{unit.CharacterName}";
            scrollview.gameObject.SetActive(true);

            return scrollview.gameObject.AddComponent<SpellViewManager>();
        }

        public override void Awake()
        {
            base.Awake();

            _levelTransforms = new List<Transform>();
            _levelContentTransforms = new List<Transform>();
            BuildHeaders(ref _levelContentTransforms, ref _levelTransforms);
            Entries = new Dictionary<string, EntryData>();
            _unit = _currentUnitProcessing;
            _multiSelected = transform.FindTargetParent("ScrollViews").FirstOrDefault(x => x.name == "MultiSelected");
            _noSpells = transform.parent.FirstOrDefault(x => x.name == "NoSpells");
            _time = DateTime.Now + TimeSpan.FromMilliseconds(0.5);
            BuildList();
            OnUnitSelectionAdd(Game.Instance.UI.SelectionManager.SelectedUnits.FirstOrDefault());
            
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
                    foreach(var spell in book.GetAllKnownSpells().Where(x => x.GetAvailableForCastCount() > 0 || x.SpellLevel == 0))
                    {
                        abilities.Add(new MechanicActionBarSlotSpontaneousSpell(spell));
                    }
                else
                {
                    foreach (var spell in book.GetKnownSpells(0))
                    {
                        abilities.Add(new MechanicActionBarSlotSpontaneousSpell(spell));
                    }

                    foreach (var spell in book.GetAllMemorizedSpells().Where(x => x.Spell.GetAvailableForCastCount() > 0))
                    {
                        abilities.Add(new MechanicActionBarSlotMemorizedSpell(spell)) ;
                    }
                }
            }

            foreach (var a in abilities)
            {
                if (!Entries.ContainsKey(a.Spell.ToString()))
                {
                    a.Unit = _unit;
                    Entries.Add(a.Spell.ToString(), InsertTransform(a, _levelContentTransforms[a.Spell.SpellLevel], _levelTransforms[a.Spell.SpellLevel])) ;
                }
            }

            foreach(var v in Entries.ToList().Select(x => x.Key).Except(abilities.Select(x => x.Spell.ToString())))
            {
                var slot = (MechanicActionBarSlotSpell)Entries[v].MSlot;
                RemoveTransform(v, slot, _levelContentTransforms[slot.Spell.SpellLevel], _levelTransforms[slot.Spell.SpellLevel]);
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

        public void HandleModDisable()
        {
            EventBus.Unsubscribe(this);
        }

        
        public void HandleViewChange()
        {
            OnUnitSelectionAdd(Game.Instance.UI.SelectionManager.SelectedUnits.FirstOrDefault());
        }
    }
}
