using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using QuickCast.UI.Builders;
using QuickCast.UI.Monos.ElementTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace QuickCast.UI.Monos.ViewControlGroup.SpellSV
{
    internal class SpellSVManager : SVManager
    {
        public UnitEntityData Unit;
        public bool HasSpells => _spellElements.Count > 0;

        private States.SortState _sortState;
        private States.SpellState _spellState;
        private bool _isInit = false;

        private List<LevelHeaderElement> _levelHeaders;
        private SortedDictionary<string, BookHeaderElement> _bookHeaders;
        private SortedDictionary<string, SpellElement> _spellElements;

        public override void Initialize()
        {
            if (_isInit) return;

            base.Initialize();

            _levelHeaders = new();
            _bookHeaders = new();
            _spellElements = new();

            for (int i = 0; i <= 10; i++)
            {
                _levelHeaders.Add(BuildUI.BuildLevelHeaderElement(_scrollRect.content, i, "ROOT", "Spells"));
            }

            foreach (var book in Unit.Spellbooks)
            {
                var b = BuildUI.BuildBookHeaderElement(_scrollRect.content, book);
                _bookHeaders.Add(b.gameObject.name, b);

                foreach (var spell in book.GetAllKnownSpells())
                {
                    AddSpellElement(spell, book);
                }
            }

            SetSort(States.SortState.Level);

            _isInit = true;
        }

        public void AddSpellElement(AbilityData spell, Spellbook book)
        {
            var key = GetKey(book, spell);
            var level = spell.SpellLevelInSpellbook ?? spell.SpellLevel;
            var se = BuildUI.BuildSpellElement(_scrollRect.content, spell, level, key);
            if (!_spellElements.ContainsKey(key))
            {
                _spellElements.Add(key, se);
                _levelHeaders[level].Add(key, se);
                _bookHeaders[GetBookKey(book)].Add(key, se);
            }
        }

        public void SetSort(States.SortState sortState)
        {
            if (_sortState == sortState) return;

            _sortState = sortState;

            switch (sortState)
            {
                case States.SortState.Level:
                    SortBy(_levelHeaders);
                    foreach (var e in _bookHeaders.Values)
                        e.Unclaim();
                    break;
                case States.SortState.Book:
                    SortBy(_bookHeaders.Values.ToList());
                    foreach (var e in _levelHeaders)
                        e.Unclaim();
                    break;
            }
        }



        public static string GetKey(Spellbook book, AbilityData spell) => $"{book.Blueprint.name}-{spell.SpellLevel}-{spell.SpellLevelInSpellbook}-{spell.Blueprint.name}";
        public static string GetBookKey(Spellbook book) => $"{book.Blueprint.Name}-Spellbook-Header";

    }
}
