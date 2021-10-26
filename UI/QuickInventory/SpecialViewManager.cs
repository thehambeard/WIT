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
using Kingmaker.UnitLogic.ActivatableAbilities;
using DG.Tweening;

namespace QuickCast.UI.QuickInventory
{
    public class SpecialViewManager : MonoBehaviour, IModEventHandler, ISelectionHandler, IViewChangeHandler
    {
        private UnitEntityData _unit;
        private static UnitEntityData _currentUnitProcessing;
        public bool _isDirty = true;
        private Dictionary<Ability, AbilityEntryData> _abilities;
        private Dictionary<ActivatableAbility, ActivatableEntryData> _activatableAbilities;
        private Transform _template;
        private DateTime _time;
        private Transform _multiSelected;
        private Transform _noSpells;
        private bool _expandAll = true;

        public int Priority => 500;

        public static SpecialViewManager CreateObject(UnitEntityData unit)
        {
            _currentUnitProcessing = unit;
            var scrollview = GameObject.Instantiate(Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate"), Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViews"), false);
            scrollview.name = $"ScrollViewSpecial{unit.CharacterName}";
            scrollview.gameObject.SetActive(true);

            var spellLevels = scrollview.Find("Viewport/Content/SpellLevel");
            var spellLevelsContent = scrollview.Find("Viewport/Content/SpellLevelContent");

            
            var t = GameObject.Instantiate(spellLevels, spellLevels.parent, false);
            var tc = GameObject.Instantiate(spellLevelsContent, spellLevelsContent.parent, false);
            tc.name = "SpecialAbilityContent";
            tc.gameObject.SetActive(false);
            tc.Find("Spell").SafeDestroy();
            t.name = "SpecialAbility";
            t.GetComponentInChildren<TextMeshProUGUI>().text = "Abilities";
            t.gameObject.SetActive(false);

            t = GameObject.Instantiate(spellLevels, spellLevels.parent, false);
            tc = GameObject.Instantiate(spellLevelsContent, spellLevelsContent.parent, false);
            tc.name = "SpecialActivatableAbilityContent";
            tc.gameObject.SetActive(false);
            tc.Find("Spell").SafeDestroy();
            t.name = "SpecialActivatableAbility";
            t.GetComponentInChildren<TextMeshProUGUI>().text = "Activatable Abilities";
            t.gameObject.SetActive(false);


            spellLevels.SafeDestroy();
            spellLevelsContent.SafeDestroy();

            return scrollview.gameObject.AddComponent<SpecialViewManager>();
        }

        void Awake()
        {
            _template = Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate");
            _abilities = new Dictionary<Ability, AbilityEntryData>();
            _activatableAbilities = new Dictionary<ActivatableAbility, ActivatableEntryData>();
            _unit = _currentUnitProcessing;
            _multiSelected = transform.FindTargetParent("ScrollViews").FirstOrDefault(x => x.name == "MultiSelected");
            _noSpells = transform.parent.FirstOrDefault(x => x.name == "NoSpells");
            _time = DateTime.Now + TimeSpan.FromMilliseconds(0.5);
            BuildList(); 
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
        public void BuildList()
        {
            
            foreach (var a in _unit.Abilities.Enumerable)
            {
                if (!_abilities.ContainsKey(a))
                {
                    _abilities.Add(a, InsertSpellTransform(a));
                }
            }

            foreach (var v in _abilities.ToList().Select(x => x.Key).Except(_unit.Abilities.Enumerable).Reverse())
            {
                RemoveSpellTransform(v);
            }

            foreach (var a in _unit.ActivatableAbilities.Enumerable)
            {
                if (!_activatableAbilities.ContainsKey(a))
                {
                    _activatableAbilities.Add(a, InsertSpellTransform(a));
                }
            }

            foreach (var v in _activatableAbilities.ToList().Select(x => x.Key).Except(_unit.ActivatableAbilities.Enumerable).Reverse())
            {
                RemoveSpellTransform(v);
            }

            SortTransforms();
        }

        private void RemoveSpellTransform(Ability ability)
        {
            var parentTransform = transform.Find($"Viewport/Content/SpecialAbilityContent");
            var sibTransform = transform.Find($"Viewport/Content/SpecialAbility");
            GameObject.DestroyImmediate(_abilities[ability].Transform.gameObject);
            _abilities.Remove(ability);
            if (parentTransform.childCount <= 0)
            {
                parentTransform.gameObject.SetActive(false);
                sibTransform.gameObject.SetActive(false);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parentTransform);
        }

        private void RemoveSpellTransform(ActivatableAbility ability)
        {
            var parentTransform = transform.Find($"Viewport/Content/SpecialActivatableAbilityContent");
            var sibTransform = transform.Find($"Viewport/Content/SpecialActivatableAbility");
            GameObject.DestroyImmediate(_activatableAbilities[ability].Transform.gameObject);
            _activatableAbilities.Remove(ability);
            if (parentTransform.childCount <= 0)
            {
                parentTransform.gameObject.SetActive(false);
                sibTransform.gameObject.SetActive(false);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parentTransform);
        }

        private void SortTransforms()
        {
            int i = 0;
            foreach (var t in _abilities.OrderBy(x => x.Key.Name))
                t.Value.Transform.SetSiblingIndex(i++);
            i = 0;
            foreach (var t in _activatableAbilities.OrderBy(x => x.Key.Name))
                t.Value.Transform.SetSiblingIndex(i++);
        }

        private void RunCommand(AbilityEntryData entry)
        {
            entry.Slot.OnClick();
        }

        private void RunCommand(ActivatableEntryData entry)
        {
            entry.Slot.OnClick();       
        }

        private AbilityEntryData InsertSpellTransform(Ability ability)
        {

            var parentTransform = transform.Find($"Viewport/Content/SpecialAbilityContent");
            var sibTransform = transform.Find($"Viewport/Content/SpecialAbility");
            var spellContentTransform = GameObject.Instantiate(_template.Find("Viewport/Content/SpellLevelContent/Spell"), parentTransform, false);
            spellContentTransform.name = ability.Name;
            var text = spellContentTransform.Find("SpellText").GetComponent<TextMeshProUGUI>();
            text.text = ability.Name;
            text.color = new Color(.31f, .31f, .31f);

            var button = spellContentTransform.GetComponentInChildren<Button>();
            var entry = new AbilityEntryData()
            {
                Transform = spellContentTransform,
                Button = button,
                DCText = spellContentTransform.FirstOrDefault(x => x.name == "DCText").GetComponent<TextMeshProUGUI>(),
                UsesText = spellContentTransform.FirstOrDefault(x => x.name == "UsesText").GetComponent<TextMeshProUGUI>(),
                Slot = new MechanicActionBarSlotAbility()
                {
                    Ability = ability.Data,
                    Unit = _unit
                }
            };
            button.onClick.AddListener(() => RunCommand(entry));
            parentTransform.gameObject.SetActive(true);
            sibTransform.gameObject.SetActive(true);


            return entry;
        }

        private ActivatableEntryData InsertSpellTransform(ActivatableAbility ability)
        {

            var parentTransform = transform.Find($"Viewport/Content/SpecialActivatableAbilityContent");
            var sibTransform = transform.Find($"Viewport/Content/SpecialActivatableAbility");
            var spellContentTransform = GameObject.Instantiate(_template.Find("Viewport/Content/SpellLevelContent/Spell"), parentTransform, false);
            spellContentTransform.name = ability.Name;
            var text = spellContentTransform.Find("SpellText").GetComponent<TextMeshProUGUI>();
            text.text = ability.Name;
            text.color = new Color(.31f, .31f, .31f);

            var button = spellContentTransform.GetComponentInChildren<Button>();
            var entry = new ActivatableEntryData()
            {
                Transform = spellContentTransform,
                Button = button,
                DCText = spellContentTransform.FirstOrDefault(x => x.name == "DCText").GetComponent<TextMeshProUGUI>(),
                UsesText = spellContentTransform.FirstOrDefault(x => x.name == "UsesText").GetComponent<TextMeshProUGUI>(),
                Slot = new MechanicActionBarSlotActivableAbility()
                {
                    ActivatableAbility = ability,
                    Unit = _unit
                }

            };
            button.onClick.AddListener(() => RunCommand(entry));
            parentTransform.gameObject.SetActive(true);
            sibTransform.gameObject.SetActive(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parentTransform);

            return entry;
        }

        private void HandleLevelClick(Button button, int forceState = 0)
        {
            var toggleExpand = button.transform.parent.GetChild(2);
            bool active;
            switch (forceState)
            {
                case 1:
                    active = true;
                    break;
                case 2:
                    active = false;
                    break;
                default:
                    active = !button.transform.parent.parent.GetChild(button.transform.parent.GetSiblingIndex() + 1).gameObject.activeSelf;
                    break;
            }
            button.transform.parent.parent.GetChild(button.transform.parent.GetSiblingIndex() + 1).gameObject.SetActive(active);
            if (active)
                toggleExpand.DORotate(new Vector3(0f, 0f, 0f), .25f).SetUpdate(true);
            else
                toggleExpand.DORotate(new Vector3(0, 0, 180f), .25f).SetUpdate(true);
        }

        public void ToggleCollapseExpandAll()
        {
            foreach (var button in transform.GetComponentsInChildren<Button>().Where(x => x.name == "SpellLevelBackground"))
            {
                HandleLevelClick(button, (_expandAll) ? 1 : 2);
            }
            _expandAll = !_expandAll;
        }

        public void OnUnitSelectionAdd(UnitEntityData selected)
        {
            if (!Game.Instance.UI.SelectionManager.IsSingleSelected)
            {
                _multiSelected.gameObject.SetActive(true);
                _multiSelected.SetAsLastSibling();
                return;
            }
            

            if (Game.Instance.UI.SelectionManager.FirstSelectUnit == _unit && Mod.Core.UI.MainWindowManager.CurrentViewPort == MainWindowManager.ViewPortType.Special)
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
            foreach(var t in _activatableAbilities)
            {
                if (t.Value.Slot.IsActive())
                    t.Value.DCText.text = "X";
                else
                    t.Value.DCText.text = "";
                if (t.Value.Slot.ActivatableAbility.ResourceCount == null)
                    t.Value.UsesText.text = "-";
                else
                    t.Value.UsesText.text = t.Value.Slot.ActivatableAbility.ResourceCount.ToString();

            }

            foreach(var t in _abilities)
            {
                if (t.Value.Slot.Ability.GetAvailableForCastCount() == -1)
                    t.Value.UsesText.text = "-";
                else
                    t.Value.UsesText.text = t.Value.Slot.Ability.GetAvailableForCastCount().ToString();
                t.Value.DCText.text = t.Value.Slot.Ability.CalculateParams().DC.ToString();
            }
        }
        public void HandleViewChange()
        {
            OnUnitSelectionAdd(Game.Instance.UI.SelectionManager.SelectedUnits.FirstOrDefault());
        }
    }
}
