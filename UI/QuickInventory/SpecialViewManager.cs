using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.UnitSettings;
using Kingmaker.Utility;
using ModMaker;
using ModMaker.Utility;
using QuickCast.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static QuickCast.Main;

namespace QuickCast.UI.QuickInventory
{
    public class SpecialViewManager : ViewManager, IModEventHandler, ISelectionHandler, IViewChangeHandler
    {
        private Dictionary<string, EntryData> _abilities;
        private Dictionary<string, EntryData> _activatableAbilities;
        private DateTime _time;

        public int Priority => 500;

        public static SpecialViewManager CreateObject(UnitEntityData unit)
        {
            var scrollview = GameObject.Instantiate(Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate"), Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViews"), false);
            scrollview.name = $"ScrollViewSpecial{unit.CharacterName}";
            scrollview.gameObject.SetActive(true);

            var scrollViewMono = scrollview.gameObject.AddComponent<SpecialViewManager>();
            scrollViewMono._unit = unit;
            scrollViewMono._viewPortType = MainWindowManager.ViewPortType.Special;

            return scrollViewMono;
        }

        public override void Start()
        {
            base.Start();
            _abilities = new Dictionary<string, EntryData>();
            _activatableAbilities = new Dictionary<string, EntryData>();
            _time = DateTime.Now + TimeSpan.FromMilliseconds(0.5);

            BuildAbilityHeaders();
            BuildList();
            RestoreHeaders();
            foreach (var button in transform.GetComponentsInChildren<Button>().Where(x => x.name == "SpellLevelBackground"))
            {
                button.onClick.AddListener(() => HandleLevelClick(button));
            }

            EventBus.Subscribe(this);
            transform.gameObject.SetActive(false);
        }

        void Update()
        {
            if (DateTime.Now > _time)
            {
                BuildList();
                _time = DateTime.Now + TimeSpan.FromMilliseconds(750f);
                UpdateUsesAndDC();
            }
        }

        public void BuildAbilityHeaders()
        {
            _levelTransforms = new List<Transform>();
            _levelContentTransforms = new List<Transform>();

            var spellLevels = transform.Find("Viewport/Content/SpellLevel");
            var spellLevelsContent = transform.Find("Viewport/Content/SpellLevelContent");
            bool createStates = false;

            if (SetWrap.HeaderStates == null)
            {
                SetWrap.HeaderStates = new SerializableDictionary<MainWindowManager.ViewPortType, List<bool>>();
            }

            if (!SetWrap.HeaderStates.ContainsKey(_viewPortType))
            {
                SetWrap.HeaderStates.Add(_viewPortType, new List<bool>());
                createStates = true;
            }

            if (createStates)
                SetWrap.HeaderStates[_viewPortType].Add(false);
            var t = GameObject.Instantiate(spellLevels, spellLevels.parent, false);
            var tc = GameObject.Instantiate(spellLevelsContent, spellLevelsContent.parent, false);
            tc.name = "SpecialAbilityContent";
            tc.gameObject.SetActive(false);
            tc.Find("Spell").SafeDestroy();
            t.name = "SpecialAbility";
            t.GetComponentInChildren<TextMeshProUGUI>().text = "Abilities";
            t.gameObject.SetActive(false);
            _levelTransforms.Add(t);
            _levelContentTransforms.Add(tc);

            if (createStates)
                SetWrap.HeaderStates[_viewPortType].Add(false);
            t = GameObject.Instantiate(spellLevels, spellLevels.parent, false);
            tc = GameObject.Instantiate(spellLevelsContent, spellLevelsContent.parent, false);
            tc.name = "SpecialActivatableAbilityContent";
            tc.gameObject.SetActive(false);
            tc.Find("Spell").SafeDestroy();
            t.name = "SpecialActivatableAbility";
            t.GetComponentInChildren<TextMeshProUGUI>().text = "Activatable Abilities";
            t.gameObject.SetActive(false);
            _levelTransforms.Add(t);
            _levelContentTransforms.Add(tc);

            spellLevels.SafeDestroy();
            spellLevelsContent.SafeDestroy();
        }

        public void BuildList()
        {

            foreach (var a in _unit.Abilities.Enumerable)
            {
                if (!_abilities.ContainsKey(a.ToString()))
                {
                    _abilities.Add(a.ToString(), InsertTransform(new MechanicActionBarSlotAbility() { Ability = a.Data, Unit = _unit }, a.Name, _levelContentTransforms[0], _levelTransforms[0]));
                }
            }

            foreach (var v in _abilities.ToList().Select(x => x.Key).Except(_unit.Abilities.Enumerable.Select(x => x.ToString())))
            {
                RemoveTransform(v, _abilities, _levelContentTransforms[0], _levelTransforms[0]);
            }
            foreach (var a in _unit.ActivatableAbilities.Enumerable)
            {
                if (!_activatableAbilities.ContainsKey(a.ToString()))
                {
                    _activatableAbilities.Add(a.ToString(), InsertTransform(new MechanicActionBarSlotActivableAbility() { ActivatableAbility = a, Unit = _unit }, a.Name, _levelContentTransforms[1], _levelTransforms[1]));
                }
            }

            foreach (var v in _activatableAbilities.ToList().Select(x => x.Key).Except(_unit.ActivatableAbilities.Enumerable.Select(x => x.ToString())))
            {
                RemoveTransform(v, _activatableAbilities, _levelContentTransforms[1], _levelTransforms[1]);
            }

            SortTransforms(_abilities);
            SortTransforms(_activatableAbilities);

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_levelContentTransforms[0]);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_levelContentTransforms[1]);
        }

        public void OnUnitSelectionAdd(UnitEntityData selected)
        {
            if (Game.Instance.UI.SelectionManager.SelectedUnits.Count() != 1)
            {
                _multiSelected.gameObject.SetActive(true);
                _multiSelected.SetAsLastSibling();
                return;
            }


            if (Game.Instance.UI.SelectionManager.SelectedUnits.FirstOrDefault<UnitEntityData>() == _unit && Mod.Core.UI.MainWindowManager.CurrentViewPort == _viewPortType)
            {
                bool hasAbility = _unit.Abilities.Enumerable.Any();
                bool hasActivatable = _unit.ActivatableAbilities.Enumerable.Any();

                if (!hasAbility && !hasActivatable)
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

        public void UpdateUsesAndDC()
        {
            foreach (var t in _activatableAbilities)
            {
                var Slot = (MechanicActionBarSlotActivableAbility)t.Value.MSlot;
                if (Slot.IsActive())
                    t.Value.DCText.text = "X";
                else
                    t.Value.DCText.text = "";
                if (Slot.ActivatableAbility.ResourceCount == null)
                    t.Value.UsesText.text = "-";
                else
                    t.Value.UsesText.text = Slot.ActivatableAbility.ResourceCount.ToString();

            }

            foreach (var t in _abilities)
            {
                var Slot = (MechanicActionBarSlotAbility)t.Value.MSlot;
                if (Slot.Ability.GetAvailableForCastCount() == -1)
                    t.Value.UsesText.text = "-";
                else
                    t.Value.UsesText.text = Slot.Ability.GetAvailableForCastCount().ToString();
                t.Value.DCText.text = Slot.Ability.CalculateParams().DC.ToString();
            }
        }
        public void HandleViewChange()
        {
            OnUnitSelectionAdd(Game.Instance.UI.SelectionManager.SelectedUnits.FirstOrDefault());
        }
    }
}
