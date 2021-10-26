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
    public class SpellViewManager : MonoBehaviour, IModEventHandler, ISelectionHandler, IViewChangeHandler
    {
        private UnitEntityData _unit;
        private static UnitEntityData _currentUnitProcessing;
        public bool _isDirty = true;
        private Dictionary<AbilityData, SpellEntryData> _spells;
        private Transform _template;
        private DateTime _time;
        private Transform _multiSelected;
        private Transform _noSpells;
        private bool _expandAll = true;

        public int Priority => 500;

        public static SpellViewManager CreateObject(UnitEntityData unit)
        {
            _currentUnitProcessing = unit;
            var scrollview = GameObject.Instantiate(Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate"), Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViews"), false);
            scrollview.name = $"ScrollViewSpells{unit.CharacterName}";
            scrollview.gameObject.SetActive(true);

            var spellLevels = scrollview.Find("Viewport/Content/SpellLevel");
            var spellLevelsContent = scrollview.Find("Viewport/Content/SpellLevelContent");

            for (int i = 0; i <= 10; i++)
            {
                var t = GameObject.Instantiate(spellLevels, spellLevels.parent, false);
                var tc = GameObject.Instantiate(spellLevelsContent, spellLevelsContent.parent, false);
                tc.name = $"SpellLevelContent{i}";
                tc.gameObject.SetActive(false);
                tc.Find("Spell").SafeDestroy();
                t.name = $"SpellLevel{i}";
                t.GetComponentInChildren<TextMeshProUGUI>().text = $"Level {i} Spells";
                t.gameObject.SetActive(false);
            }

            spellLevels.SafeDestroy();
            spellLevelsContent.SafeDestroy();

            return scrollview.gameObject.AddComponent<SpellViewManager>();
        }

        void Awake()
        {
            _template = Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate");
            _spells = new Dictionary<AbilityData, SpellEntryData>();
            _unit = _currentUnitProcessing;
            _multiSelected = transform.FindTargetParent("ScrollViews").FirstOrDefault(x => x.name == "MultiSelected");
            _noSpells = transform.parent.FirstOrDefault(x => x.name == "NoSpells");
            _time = DateTime.Now + TimeSpan.FromMilliseconds(0.5);
            BuildList();
            OnUnitSelectionAdd(Game.Instance.UI.SelectionManager.SelectedUnits.FirstOrDefault());
            foreach (var button in transform.GetComponentsInChildren<Button>().Where(x => x.name == "SpellLevelBackground"))
            {
                button.onClick.AddListener(() => HandleLevelClick(button));
            }
            EventBus.Subscribe(this);
            transform.gameObject.SetActive(false);
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
            List<AbilityData> abilities = new List<AbilityData>();

            foreach (var book in _unit.Spellbooks)
            {
                if (book.Blueprint.Spontaneous)
                    foreach(var spell in book.GetAllKnownSpells().Where(x => x.GetAvailableForCastCount() > 0 || x.SpellLevel == 0))
                    {
                        abilities.Add(spell);
                    }
                else
                {
                    foreach (var spell in book.GetKnownSpells(0))
                    {
                        abilities.Add(spell);
                    }

                    foreach (var spell in book.GetAllMemorizedSpells().Where(x => x.Spell.GetAvailableForCastCount() > 0))
                    {
                        abilities.Add(spell.Spell);
                    }
                }
            }

            foreach (var a in abilities)
            {
                if (!_spells.ContainsKey(a))
                {
                    _spells.Add(a, InsertSpellTransform(a));
                }
            }

            foreach (var v in _spells.ToList().Select(x => x.Key).Except(abilities).Reverse())
            {
                RemoveSpellTransform(v);
            }

            SortTransforms();
        }

        private void RemoveSpellTransform(AbilityData ability)
        {
            var parentTransform = transform.Find($"Viewport/Content/SpellLevelContent{ability.SpellLevel}");
            var sibTransform = transform.Find($"Viewport/Content/SpellLevel{ability.SpellLevel}");
            GameObject.DestroyImmediate(_spells[ability].Transform.gameObject);
            _spells.Remove(ability);
            if (parentTransform.childCount <= 0)
            {
                parentTransform.gameObject.SetActive(false);
                sibTransform.gameObject.SetActive(false);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) parentTransform);
        }

        private void SortTransforms()
        {
            int i = 0;
            foreach (var t in _spells.OrderBy(x => x.Key.Name))
                t.Value.Transform.SetSiblingIndex(i++);
        }

        private void RunCommand(SpellEntryData entry)
        {
            Mod.Debug(entry.Data.TargetAnchor.ToString());

            if(entry.Data.IsSpontaneous || entry.Data.SpellLevel == 0)
            {
                MechanicActionBarSlotSpontaneousSpell spon = new MechanicActionBarSlotSpontaneousSpell(entry.Data);
                spon.Unit = _unit;
                spon.OnClick();
            }
            else
            {
                MechanicActionBarSlotMemorizedSpell mem = new MechanicActionBarSlotMemorizedSpell(entry.Data.SpellSlot);
                mem.Unit = _unit;
                mem.OnClick();
            }
        }

        private SpellEntryData InsertSpellTransform(AbilityData ability)
        {
            
            var parentTransform = transform.Find($"Viewport/Content/SpellLevelContent{ability.SpellLevel}");
            var sibTransform = transform.Find($"Viewport/Content/SpellLevel{ability.SpellLevel}"); 
            var spellContentTransform = GameObject.Instantiate(_template.Find("Viewport/Content/SpellLevelContent/Spell"), parentTransform, false);
            spellContentTransform.name = ability.Name;
            var text = spellContentTransform.Find("SpellText").GetComponent<TextMeshProUGUI>();
            text.text = ability.Name;
            text.color = new Color(.31f, .31f, .31f);

            var button = spellContentTransform.GetComponentInChildren<Button>();
            var entry = new SpellEntryData()
            {
                Transform = spellContentTransform,
                Button = button,
                Data = ability,
                DCText = spellContentTransform.FirstOrDefault(x => x.name == "DCText").GetComponent<TextMeshProUGUI>(),
                UsesText = spellContentTransform.FirstOrDefault(x => x.name == "UsesText").GetComponent<TextMeshProUGUI>()

            };
            button.onClick.AddListener(() => RunCommand(entry));
            parentTransform.gameObject.SetActive(true);
            sibTransform.gameObject.SetActive(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parentTransform);

            return entry;
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

        public void UpdateUsesAndDC()
        {
            foreach (var kvp in _spells)
            {
                if (kvp.Key.SpellLevel == 0)
                    continue;
                if (kvp.Key.IsSpontaneous && kvp.Key.Spellbook != null)
                    kvp.Value.UsesText.text = kvp.Key.Spellbook.GetSpontaneousSlots(kvp.Key.SpellLevel).ToString();
                else if (kvp.Key.SpellSlot != null)
                    kvp.Value.UsesText.text = kvp.Key.Spellbook.GetAvailableForCastSpellCount(kvp.Key).ToString();
                kvp.Value.DCText.text = kvp.Key.CalculateParams().DC.ToString();
            }
        }
        public void HandleViewChange()
        {
            OnUnitSelectionAdd(Game.Instance.UI.SelectionManager.SelectedUnits.FirstOrDefault());
        }
    }
}
