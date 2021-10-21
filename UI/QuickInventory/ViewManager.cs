//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Kingmaker;
//using Kingmaker.EntitySystem.Entities;
//using Kingmaker.Utility.UnitDescription;
//using UnityEngine;
//using TMPro;
//using UnityEngine.UI;
//using WIT.Utilities;
//using Owlcat.Runtime.Core.Logging;
//using static WIT.Main;
//using Kingmaker.UnitLogic;
//using Kingmaker.PubSubSystem;
//using Kingmaker.UnitLogic.Abilities;
//using System.Diagnostics;
//using Kingmaker.UnitLogic.Commands;
//using Kingmaker.UI.Common;
//using Kingmaker.UnitLogic.Abilities.Blueprints;
//using Kingmaker.UI;
//using Kingmaker.Items.Slots;
//using Kingmaker.Items;
//using Kingmaker.RuleSystem.Rules;
//using DG.Tweening;
//using Kingmaker.Blueprints.Items.Equipment;
//using static WIT.UI.QuickInventory.MainWindowManager;

//namespace WIT.UI.QuickInventory
//{
//    public class ViewManager : MonoBehaviour, IAbilityExecutionProcessHandler, ISubscriber, IGlobalSubscriber, ISelectionHandler, IPartyCombatHandler, ISpellBookCustomSpell, ISpellBookRest, 
//        /*IPlayerAbilitiesHandler,*/ ISpellBookUIHandler, IUnitEquipmentHandler, ITurnBasedModeHandler, IUIElement, ISelectionManagerUIHandler, IViewChangeHandler
//    {
//        protected UnitEntityData _unit;
//        protected static UnitEntityData _currentUnitProcessing;
//        protected static ViewPortType _currentViewProcessing;
//        protected ViewPortType _viewPortType;
//        protected List<AbilityData> _spells = new List<AbilityData>();

//        private Dictionary<string, RectTransform> _templateLookup = new Dictionary<string, RectTransform>();
//        protected Dictionary<AbilityData, EntryData> _abilityLookup = new Dictionary<AbilityData, EntryData>();

//        public bool IsDirty = true;
//        private Transform _content;
//        private Transform _multiSelected;
//        private Transform _noSpells;

//        protected virtual void Awake()
//        {
//            _content = transform.FirstOrDefault(x => x.name == "Content");
//            _multiSelected = transform.FindTargetParent("ScrollViews").FirstOrDefault(x => x.name == "MultiSelected");
//            _noSpells = transform.parent.FirstOrDefault(x => x.name == "NoSpells");
//            var template = Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate");
//            foreach(RectTransform transform in template.GetChildRecursive())
//                _templateLookup.Add(transform.name, transform);
//            BuildTransforms();
//            EventBus.Subscribe(this);
//        }

//        //public void BuildList()
//        //{
//        //    List<AbilityData> abilities = new List<AbilityData>();
//        //    _spells.Clear();

//        //    foreach (var book in _unit.Spellbooks)
//        //    {
                
//        //        if (book.Blueprint.Spontaneous)
//        //            abilities.AddRange(book.GetAllKnownSpells().ToList());
//        //        else
//        //        {
//        //            abilities.AddRange(book.GetKnownSpells(0).ToList());
//        //            abilities.AddRange(book.GetAllMemorizedSpells().Select(x => x.Spell).ToList());
//        //        }
//        //    }
//        //    foreach (var ability in abilities)
//        //    {
//        //        _spells.Add(ability);
//        //    }
//        //}

//        public void BuildTransforms()
//        {
            
//            int currentLevel = -1;
//            RectTransform spellLevelContent = null;

//            foreach(Transform t in _content)
//                GameObject.Destroy(t.gameObject);

//            foreach (var spell in _spells.OrderBy(x => x.SpellLevel).ThenBy(x => x.Name))
//            {
//                if (!_abilityLookup.ContainsKey(spell))
//                {
//                    if (currentLevel != spell.SpellLevel)
//                    {
//                        var spellLevel = GameObject.Instantiate(_templateLookup["SpellLevel"], _content, false);
//                        spellLevel.name = $"SpellLevel";
//                        spellLevelContent = GameObject.Instantiate(_templateLookup["SpellLevelContent"], _content, false);
//                        spellLevelContent.name = $"SpellLevelContent";
//                        foreach (Transform t in spellLevelContent)
//                            GameObject.Destroy(t.gameObject);
//                        var levelButton = spellLevel.GetComponentInChildren<Button>();
//                        levelButton.onClick.AddListener(() => HandleLevelClick(levelButton));
//                        spellLevel.GetComponentInChildren<TextMeshProUGUI>().text = $"Level {spell.SpellLevel} spells";
//                        currentLevel = spell.SpellLevel;
//                    }
//                    var currentSpell = GameObject.Instantiate(_templateLookup["Spell"], spellLevelContent, false);
//                    var entryButton = currentSpell.GetComponentInChildren<Button>();
//                    entryButton.onClick.AddListener(() => HandleSpellClick());
//                    currentSpell.name = "Spell";
//                    var currentSpellText = currentSpell.GetChild(1).GetComponent<TextMeshProUGUI>();
//                    var currentUsesText = currentSpell.GetChild(3).GetComponentInChildren<TextMeshProUGUI>();
//                    var currentDCText = currentSpell.GetChild(4).GetComponentInChildren<TextMeshProUGUI>();

//                    currentSpellText.color = new Color(0.1027f, 0f, 0.0345f, 1f);
//                    currentSpellText.alignment = TextAlignmentOptions.MidlineLeft;
//                    currentSpellText.text = spell.Name;
//                    _abilityLookup.Add(spell, new EntryData() { Transform = currentSpell, Button = entryButton, Data = spell, UsesText = currentUsesText, DCText = currentDCText });
//                }
//            }

//            ValidateTransforms();
            
//        }


//        private void ValidateTransforms()
//        {
//            foreach (var spell in _spells.OrderBy(x => x.SpellLevel).ThenBy(x => x.Name))
//            {
//                if (_abilityLookup.ContainsKey(spell) && ((spell.IsSpontaneous && spell.Spellbook.GetSpontaneousSlots(spell.SpellLevel) <= 0) || (!spell.IsSpontaneous && spell.SpellSlot != null && spell.SpellSlot.BusySlotsCount <= 0)) && spell.SpellLevel != 0)
//                {
//                    if (_abilityLookup[spell].Transform.parent.childCount <= 1)
//                    {
//                        _abilityLookup[spell].Transform.parent.gameObject.SetActive(false);
//                        _abilityLookup[spell].Transform.parent.parent.GetChild(_abilityLookup[spell].Transform.parent.GetSiblingIndex() - 1).gameObject.SetActive(false);
//                    }
//                    GameObject.DestroyImmediate(_abilityLookup[spell].Transform.gameObject);
//                    _abilityLookup.Remove(spell);
//                }
//            }
//            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_content);
//        }

//        //public void UpdateUsesAndDC()
//        //{
//        //    foreach(var kvp in _abilityLookup)
//        //    {
//        //        if (kvp.Key.SpellLevel == 0)
//        //            continue;
//        //        if (kvp.Key.IsSpontaneous && kvp.Key.Spellbook != null)
//        //            kvp.Value.UsesText.text = kvp.Key.Spellbook.GetSpontaneousSlots(kvp.Key.SpellLevel).ToString();
//        //        else if (kvp.Key.SpellSlot != null)
//        //            kvp.Value.UsesText.text = kvp.Key.SpellSlot.BusySlotsCount.ToString();
//        //        kvp.Value.DCText.text = kvp.Key.CalculateParams().DC.ToString();
//        //    }
//        //}

//        private void HandleSpellClick()
//        {
//            IsDirty = true;
//        }

//        private void HandleLevelClick(Button button)
//        {
//            Mod.Warning(button.transform.GetSiblingIndex());

//            bool active = true;

//            var toggleExpand = button.transform.parent.GetChild(2);
//            foreach (Transform t in button.transform.parent.parent.GetChild(button.transform.parent.GetSiblingIndex() + 1))
//            {
//                var cg = t.GetComponent<CanvasGroup>();
//                if (t.gameObject.activeSelf)
//                {
//                    cg.alpha = 0;
//                    t.gameObject.SetActive(false);
//                    active = false;
//                }
//                else
//                {
//                    cg.DOFade(1f, .5f).SetUpdate(true);
//                    t.gameObject.SetActive(true);
//                    active = true;
//                }
//            }

//            if (active)
//                toggleExpand.DORotate(new Vector3(0f, 0f, 0f), .25f).SetUpdate(true);
//            else
//                toggleExpand.DORotate(new Vector3(0, 0, 180f), .25f).SetUpdate(true);

//            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_content);
//        }

//        protected virtual void Update()
//        {
//            if (!IsDirty) return;
//            BuildTransforms();
//            OnUnitSelectionAdd(_unit);
//            IsDirty = false;
//        }

//        public void HandleOnClick(Button button)
//        {
//            IsDirty = true;
            
//            //Game.Instance.SelectedAbilityHandler.SetAbility(_spellData[button.name]);

//            //UIUtility.GetCurrentCharacter().Commands.Run(new UnitUseAbility(_spellData[button.name], UIUtility.GetCurrentCharacter()));
//        }


//        public void HandleExecutionProcessStart(AbilityExecutionContext context)
//        {
//            IsDirty = true;
//        }

//        public void HandleExecutionProcessEnd(AbilityExecutionContext context)
//        {
//            // Not Needed
//        }

//        //public void HandleAbilityAdded(Ability ability)
//        //{
//        //    IsDirty = true;
//        //}

//        //public void HandleAbilityRemoved(Ability ability)
//        //{
//        //    IsDirty = true;
//        //}

//        public void HandleMemorizedSpell(AbilityData data, UnitDescriptor owner)
//        {
//            IsDirty = true;
//        }

//        public void HandleForgetSpell(AbilityData data, UnitDescriptor owner)
//        {
//            IsDirty = true;
//        }

//        public void HandleEquipmentSlotUpdated(ItemSlot slot, ItemEntity previousItem)
//        {
//            IsDirty = true;
//        }

//        public void HandleSurpriseRoundStarted()
//        {
//            IsDirty = true;
//        }

//        public void HandleRoundStarted(int round)
//        {
//            IsDirty = true;
//        }

//        public void HandleTurnStarted(UnitEntityData unit)
//        {
//            IsDirty = true;
//        }

//        public void HandleUnitControlChanged(UnitEntityData unit)
//        {
//            IsDirty = true;
//        }

//        public void HandleUnitNotSurprised(UnitEntityData unit, RuleSkillCheck perceptionCheck)
//        {
//            IsDirty = true;
//        }

//        public void Initialize()
//        {
//            IsDirty = true;
//        }

//        public void Dispose()
//        {
//            IsDirty = true;
//        }

//        public void OnUnitSelectionAdd(UnitEntityData selected)
//        {
//            if (!Game.Instance.UI.SelectionManager.IsSingleSelected)
//            {
//                _multiSelected.gameObject.SetActive(true);
//                _multiSelected.SetAsLastSibling();
//            }
//            else if (Game.Instance.UI.SelectionManager.FirstSelectUnit == _unit && Mod.Core.UI.MainWindowManager.CurrentViewPort == _viewPortType)
//            {
//                bool hasSpells = false;

//                switch (Mod.Core.UI.MainWindowManager.CurrentViewPort)
//                {
//                    case ViewPortType.Spells:
//                        foreach (var sb in selected.Spellbooks)
//                            hasSpells = sb.GetAllKnownSpells().Any();
//                        break;
//                    case ViewPortType.Potions:
//                        foreach (var item in Game.Instance.Player.Inventory.Items)
//                        {
//                            hasSpells = hasSpells || EnsureItem(item, UsableItemType.Potion);
//                        }
//                        break;
//                }
//                if (!hasSpells)
//                {
//                    _noSpells.gameObject.SetActive(true);
//                    _noSpells.SetAsLastSibling();
//                }
//                else
//                    transform.SetAsLastSibling();
//            }
//        }
//        private bool EnsureItem(ItemEntity item, UsableItemType type)
//        {
//            bool hasSpells = false;
//            if (item is ItemEntityUsable usable)
//            {
//                if (usable.Blueprint.Type == UsableItemType.Potion)
//                {
//                    var blueprint = (BlueprintItemEquipment)item.Blueprint;
//                    item.Ability = (Ability)_unit.Descriptor.AddFact(blueprint.Ability, null, null);
//                    if (item.Ability != null) hasSpells = true;
//                    _unit.Descriptor.RemoveFact(blueprint.Ability);
//                    item.Ability = null;
//                }
//            }
//            return hasSpells;
//        }
//        public void OnUnitSelectionRemove(UnitEntityData selected)
//        {
//        }

//        public void HandlePartyCombatStateChanged(bool inCombat)
//        {
//            IsDirty = true;
//        }

//        public void AddSpellHandler(AbilityData ability)
//        {
//            IsDirty = true;
//        }

//        public void RemoveSpellHandler(AbilityData ability)
//        {
//            IsDirty = true;
//        }

//        public void OnSpellBookRestHandler(UnitEntityData unit)
//        {
//            IsDirty = true;
//        }

//        public void HandleSwitchSelectionUnitInGroup()
//        {
//        }

//        public void HandleViewChange()
//        {
//            IsDirty = true;
//        }
//    }
//}
