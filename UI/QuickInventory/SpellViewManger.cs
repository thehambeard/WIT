using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Utility.UnitDescription;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using WIT.Utilities;
using Owlcat.Runtime.Core.Logging;
using static WIT.Main;
using Kingmaker.UnitLogic;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Abilities;
using System.Diagnostics;
using Kingmaker.UnitLogic.Commands;
using Kingmaker.UI.Common;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UI;
using Kingmaker.Items.Slots;
using Kingmaker.Items;
using Kingmaker.RuleSystem.Rules;
using DG.Tweening;

namespace WIT.UI.QuickInventory
{
    class SpellViewManager : MonoBehaviour, IAbilityExecutionProcessHandler, ISubscriber, IGlobalSubscriber, ISelectionHandler, IPartyCombatHandler, ISpellBookCustomSpell, ISpellBookRest, 
        IPlayerAbilitiesHandler, ISpellBookUIHandler, IUnitEquipmentHandler, ITurnBasedModeHandler, IUIElement
    {
        private UnitEntityData _unit;
        private static UnitEntityData _currentUnitProcessing;
        
        private List<AbilityData> _spells = new List<AbilityData>();

        private Dictionary<string, RectTransform> _templateLookup = new Dictionary<string, RectTransform>();
        private Dictionary<AbilityData, EntryData> _abilityLookup = new Dictionary<AbilityData, EntryData>();

        private bool _isDirty;
        private Transform _content;

        public static SpellViewManager CreateObject(UnitEntityData unit)
        {
            _currentUnitProcessing = unit;
            var scrollview = GameObject.Instantiate(Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate"), Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViews"), false);
            scrollview.name = $"ScrollViewSpells{unit.CharacterName}";
            scrollview.gameObject.SetActive(true);
            return scrollview.gameObject.AddComponent<SpellViewManager>();
        }

        void Awake()
        {
            _content = transform.FirstOrDefault(x => x.name == "Content");
            var template = Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate");

            _unit = _currentUnitProcessing;

            foreach(RectTransform transform in template.GetChildRecursive())
                _templateLookup.Add(transform.name, transform);
            BuildList();
            BuildTransforms();
            EventBus.Subscribe(this);
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

        public void BuildTransforms()
        {
            
            int currentLevel = -1;
            RectTransform spellLevelContent = null;

            foreach(Transform t in _content)
                GameObject.Destroy(t.gameObject);

            foreach (var spell in _spells.OrderBy(x => x.SpellLevel).ThenBy(x => x.Name))
            {
                if (currentLevel != spell.SpellLevel)
                {
                    var spellLevel = GameObject.Instantiate(_templateLookup["SpellLevel"], _content, false);
                    spellLevel.name = $"SpellLevel";
                    spellLevelContent = GameObject.Instantiate(_templateLookup["SpellLevelContent"], _content, false);
                    spellLevelContent.name = $"SpellLevelContent";
                    foreach (Transform t in spellLevelContent)
                        GameObject.Destroy(t.gameObject);
                    var levelButton = spellLevel.GetComponentInChildren<Button>();
                    levelButton.onClick.AddListener(() => HandleLevelClick(levelButton));
                    spellLevel.GetComponentInChildren<TextMeshProUGUI>().text = $"Level {spell.SpellLevel} spells";
                    currentLevel = spell.SpellLevel;
                }
                var currentSpell = GameObject.Instantiate(_templateLookup["Spell"], spellLevelContent, false);
                var entryButton = currentSpell.GetComponentInChildren<Button>();
                entryButton.onClick.AddListener(() => HandleSpellClick());
                currentSpell.name = "Spell";
                var currentSpellText = currentSpell.GetChild(1).GetComponent<TextMeshProUGUI>();
                var currentUsesText = currentSpell.GetChild(3).GetComponentInChildren<TextMeshProUGUI>();
                var currentDCText = currentSpell.GetChild(4).GetComponentInChildren<TextMeshProUGUI>();
                currentSpellText.color = new Color(0.1027f, 0f, 0.0345f, 1f);
                currentSpellText.alignment = TextAlignmentOptions.MidlineLeft;
                currentSpellText.text = spell.Name;
                if (spell.Spellbook.Blueprint.Spontaneous && spell.SpellLevel > 0)
                    currentUsesText.text = spell.Spellbook.GetSpellsPerDay(spell.SpellLevel).ToString();
                else if (spell.SpellLevel > 0)
                    currentUsesText.text = spell.SpellSlot.BusySlotsCount.ToString();
                else
                    currentUsesText.text = "";
                _abilityLookup.Add(spell, new EntryData() { Transform = currentSpell, Button = entryButton, UsesText = currentUsesText, Data = spell, DCText = currentDCText });
            }

            ValidateTransforms();
            
        }


        private void ValidateTransforms()
        {
            BuildList();

            foreach (var spell in _spells.OrderBy(x => x.SpellLevel).ThenBy(x => x.Name))
            {
                if (spell != null && !_abilityLookup.ContainsKey(spell) && ((spell.IsSpontaneous && spell.Spellbook.GetSpontaneousSlots(spell.SpellLevel) > 0) || (!spell.IsSpontaneous && spell.SpellSlot.BusySlotsCount > 0)))
                {
                    //add it
                }
                else if (spell != null && _abilityLookup.ContainsKey(spell) && ((spell.IsSpontaneous && spell.Spellbook.GetSpontaneousSlots(spell.SpellLevel) <= 0) || (!spell.IsSpontaneous && spell.SpellSlot != null && spell.SpellSlot.BusySlotsCount <= 0)) && spell.SpellLevel != 0)
                {
                    if (_abilityLookup[spell].Transform.parent.childCount <= 1)
                    {
                        _abilityLookup[spell].Transform.parent.gameObject.SetActive(false);
                        _abilityLookup[spell].Transform.parent.parent.GetChild(_abilityLookup[spell].Transform.parent.GetSiblingIndex() - 1).gameObject.SetActive(false);
                    }
                    GameObject.DestroyImmediate(_abilityLookup[spell].Transform.gameObject);
                    _abilityLookup.Remove(spell);
                    LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_content);
                }
                else
                    UpdateUsesAndDC();
            }
        }

        public void UpdateUsesAndDC()
        {
            foreach(var kvp in _abilityLookup)
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

        private void HandleSpellClick()
        {
            _isDirty = true;
        }

        private void HandleLevelClick(Button button)
        {
            Mod.Warning(button.transform.GetSiblingIndex());

            bool active = true;

            var toggleExpand = button.transform.parent.GetChild(2);
            foreach (Transform t in button.transform.parent.parent.GetChild(button.transform.parent.GetSiblingIndex() + 1))
            {
                var cg = t.GetComponent<CanvasGroup>();
                if (t.gameObject.activeSelf)
                {
                    cg.alpha = 0;
                    t.gameObject.SetActive(false);
                    active = false;
                }
                else
                {
                    cg.DOFade(1f, .5f).SetUpdate(true);
                    t.gameObject.SetActive(true);
                    active = true;
                }
            }

            if (active)
                toggleExpand.DORotate(new Vector3(0, 0, 0), .25f).SetUpdate(true);
            else
                toggleExpand.DORotate(new Vector3(0, 0, 180f), .25f).SetUpdate(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_content);
        }

        

        void Update()
        {
            if (_isDirty)
            {
                Stopwatch sw = new Stopwatch();
                sw.Restart();

                ValidateTransforms();

                sw.Stop();
                Mod.Warning($"Update took {sw.ElapsedMilliseconds} milliseconds to complete");
                _isDirty = false;
            }
        }

        public void HandleOnClick(Button button)
        {
            _isDirty = true;
            
            //Game.Instance.SelectedAbilityHandler.SetAbility(_spellData[button.name]);

            //UIUtility.GetCurrentCharacter().Commands.Run(new UnitUseAbility(_spellData[button.name], UIUtility.GetCurrentCharacter()));
        }


        public void HandleExecutionProcessStart(AbilityExecutionContext context)
        {
            _isDirty = true;
        }

        public void HandleExecutionProcessEnd(AbilityExecutionContext context)
        {
            // Not Needed
        }

        public void HandleAbilityAdded(Ability ability)
        {
            _isDirty = true;
        }

        public void HandleAbilityRemoved(Ability ability)
        {
            _isDirty = true;
        }

        public void HandleMemorizedSpell(AbilityData data, UnitDescriptor owner)
        {
            _isDirty = true;
        }

        public void HandleForgetSpell(AbilityData data, UnitDescriptor owner)
        {
            _isDirty = true;
        }

        public void HandleEquipmentSlotUpdated(ItemSlot slot, ItemEntity previousItem)
        {
            _isDirty = true;
        }

        public void HandleSurpriseRoundStarted()
        {
            _isDirty = true;
        }

        public void HandleRoundStarted(int round)
        {
            _isDirty = true;
        }

        public void HandleTurnStarted(UnitEntityData unit)
        {
            _isDirty = true;
        }

        public void HandleUnitControlChanged(UnitEntityData unit)
        {
            _isDirty = true;
        }

        public void HandleUnitNotSurprised(UnitEntityData unit, RuleSkillCheck perceptionCheck)
        {
            _isDirty = true;
        }

        public void Initialize()
        {
            _isDirty = true;
        }

        public void Dispose()
        {
            _isDirty = true;
        }

        public void OnUnitSelectionAdd(UnitEntityData selected)
        {
            _isDirty = true;
        }

        public void OnUnitSelectionRemove(UnitEntityData selected)
        {
            _isDirty = true;
        }

        public void HandlePartyCombatStateChanged(bool inCombat)
        {
            _isDirty = true;
        }

        public void AddSpellHandler(AbilityData ability)
        {
            _isDirty = true;
        }

        public void RemoveSpellHandler(AbilityData ability)
        {
            _isDirty = true;
        }

        public void OnSpellBookRestHandler(UnitEntityData unit)
        {
            _isDirty = true;
        }
    }
}
