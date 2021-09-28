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

namespace WIT.UI.QuickInventory
{
    class SpellViewManager : MonoBehaviour, IAbilityExecutionProcessHandler, ISubscriber, IGlobalSubscriber, ISelectionHandler, IPartyCombatHandler, ISpellBookCustomSpell, ISpellBookRest, 
        IPlayerAbilitiesHandler, ISpellBookUIHandler, IUnitEquipmentHandler, ITurnBasedModeHandler, IUIElement
    {
        public UnitEntityData Unit;
        private ScrollViewBuilder svb;
        private Dictionary<string, EntryData> _spellData = new Dictionary<string, EntryData>();
        private bool _isDirty;
        public static SpellViewManager CreateObject(UnitEntityData unit)
        {
            var scrollview = new ScrollViewBuilder().ReturnEmpty($"ScrollViewSpells{unit.CharacterName}");
            scrollview.SetParent(Game.Instance.UI.Canvas.transform.Find("QuickInventory").FirstOrDefault(x => x.name == "ScrollViews"), false);
            scrollview.gameObject.SetActive(true);
            return scrollview.gameObject.AddComponent<SpellViewManager>();
        }

        void Awake()
        {
            svb = new ScrollViewBuilder();
            bool showBook, showLevel;
            RectTransform bookRect, levelRect, spellRect = null;
            foreach (var spellbook in Unit.Spellbooks)
            {
                showBook = false;
                bookRect = (RectTransform) svb.TransBookEntry(spellbook);
                bookRect.SetAllParent(transform.FirstOrDefault(x => x.name == "Content"), false);
                
                for (int i = 1; i <= 10; i++)
                {
                    showLevel = false;
                    levelRect = (RectTransform)svb.TransLevelEntry(i, spellbook);
                    levelRect.SetAllParent(transform.FirstOrDefault(x => x.name == $"SpellBookContent{spellbook.Blueprint.DisplayName}"), false);
                    if (spellbook.GetSpellSlotsCount(i) > 0 || spellbook.GetMemorizedSpells(i).Count() > 0)
                    {
                        showBook = true;
                        if (spellbook.Blueprint.Spontaneous)
                        {
                            foreach (var spell in spellbook.GetKnownSpells(i))
                            {
                                showLevel = true;
                                spellRect = (RectTransform)svb.TransSpellEntry(spell, spellbook.GetSpellSlotsCount(i));
                                SharedRectSetup(spellRect, spellbook, i, spell);
                            }
                        }
                        else
                        {
                            foreach (var spell in spellbook.GetMemorizedSpells(i))
                            {
                                showLevel = true;
                                spellRect = (RectTransform)svb.TransSpellEntry(spell);
                                SharedRectSetup(spellRect, spellbook, i, spell.Spell);
                            }
                        }
                    }
                    transform.FirstOrDefault(x => x.name == $"SpellLevel{spellbook.Blueprint.DisplayName}Level{i}").gameObject.SetActive(showLevel);
                }
                transform.FirstOrDefault(x => x.name == $"SpellBook{spellbook.Blueprint.DisplayName}").gameObject.SetActive(showBook);
            }
         }

        void Update()
        {
            if (_isDirty)
            {
                ValidateSpellList();
                _isDirty = false;
            }

        }

        private void SharedRectSetup(Transform spellRect, Spellbook spellbook, int level, AbilityData spell)
        {
            spellRect.SetParent(transform.FirstOrDefault(x => x.name == $"SpellLevelContent{spellbook.Blueprint.DisplayName}Level{level}"), false);
            var button = spellRect.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => HandleOnClick(button));
            button.name = $"Spell: {spell}";
            _spellData.Add(spellRect.name, new EntryData() {Button = button, Book = spellbook, Data = spell, Transform = spellRect});
        }

        private void ValidateSpellList()
        {
            Mod.Debug("Validating the List...");
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            sw.Start();

            var currentSpells = new Dictionary<string, EntryData>();

            Mod.Debug(Unit.CharacterName);
            foreach (var spellBook in Unit.Spellbooks)
            {
                if (spellBook.Blueprint.Spontaneous)
                {
                    foreach (var spell in spellBook.GetAllKnownSpells())
                    {
                        if (spellBook.GetSpellSlotsCount(spell.SpellLevel) > 0)
                        {
                            currentSpells.Add($"Spell: {spell}", new EntryData() { Book = spellBook, Data = spell });
                        }
                    }
                }
                else
                {
                    foreach (var spell in spellBook.GetAllMemorizedSpells())
                    {
                        currentSpells.Add($"Spell: {spell.Spell}", new EntryData() { Book = spellBook, Data = spell.Spell });
                    }
                }
            }

            Mod.Debug("Current Spells has following that our list does not.");
            foreach (var d in currentSpells.Keys.Except(_spellData.Keys))
            {
                Mod.Debug(d);
            }

            Mod.Debug("Our Spells has following that current does not.");
            foreach (var d in currentSpells.Keys.Except(_spellData.Keys))
            {
                Mod.Debug(d);
            }

            sw.Stop();
            Mod.Debug(sw.Elapsed.TotalMilliseconds.ToString());
        }

        internal class EntryData
        {
            public AbilityData Data { get; set; }
            public Spellbook Book { get; set; }
            public Transform Transform { get; set; }
            public Button Button { get; set; }
        }

        public void HandleOnClick(Button button)
        {
            Mod.Debug(button.name);
            _isDirty = true;
            AbilityData data = _spellData[button.name].Data;
            Mod.Debug(data.TargetAnchor);
            if (data.IsAvailableForCast)
            {
            }

            //Game.Instance.SelectedAbilityHandler.SetAbility(_spellData[button.name]);

            //UIUtility.GetCurrentCharacter().Commands.Run(new UnitUseAbility(_spellData[button.name], UIUtility.GetCurrentCharacter()));
        }


        public void HandleExecutionProcessStart(AbilityExecutionContext context)
        {
            _isDirty = true;
        }

        public void HandleExecutionProcessEnd(AbilityExecutionContext context)
        {
            _isDirty = true;
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
