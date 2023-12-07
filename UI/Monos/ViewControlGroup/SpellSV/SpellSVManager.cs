using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using QuickCast.UI.Builders;
using QuickCast.UI.Monos.ElementTree;
using QuickCast.Utility.Extentions;
using System.Collections.Generic;
using System.Linq;

namespace QuickCast.UI.Monos.ViewControlGroup.SpellSV
{
    internal class SpellSVManager : SVManager
    {
        public UnitEntityData Unit;
        public bool HasSpells => _spellElements.Count > 0;

        public States.SortState SortState { get; private set; }
        public States.ShowUncastableState ShowUncastableState { get; private set; }

        private bool _isInit = false;

        private List<SpellLevelHeaderElement> _levelHeaders;
        private SortedList<string, BookHeaderElement> _bookHeaders;
        private SortedList<string, SpellElement> _spellElements;

        public override void Initialize()
        {
            if (_isInit) return;

            base.Initialize();

            _levelHeaders = new();
            _bookHeaders = new();
            _spellElements = new();

            for (int i = 0; i <= 10; i++)
            {
                _levelHeaders.Add(BuildUI.BuildSpellLevelHeaderElement(_scrollRect.content, i, "ROOT", "Spells"));
            }

            foreach (var book in Unit.Spellbooks)
            {
                var b = BuildUI.BuildBookHeaderElement(_scrollRect.content, book);
                _bookHeaders.Add(b.gameObject.name, b);

                foreach (var spell in book.GetAllSpellsFromBook())
                {
                    AddSpellElement(spell, book);
                }
            }

            SetSort(States.SortState.Book);
            SetShowUncastable(States.ShowUncastableState.Hid);

            _isInit = true;
        }

        private void UpdateActive()
        {
            foreach (var book in _bookHeaders.Values)
                book.UpdateActive();

            foreach (var header in _levelHeaders)
                header.UpdateActive();

            foreach (var spell in _spellElements.Values)
                spell.UpdateActive();
        }

        public void AddSpellElement(AbilityData spell, Spellbook book)
        {
            var key = GetKey(book, spell);
            var level = spell.SpellLevelInSpellbook ?? spell.SpellLevel;

            if (!_spellElements.ContainsKey(key))
            {
                var se = BuildUI.BuildSpellElement(_scrollRect.content, spell, Unit, level, key);
                _spellElements.Add(key, se);
                _levelHeaders[level].Add(key, se);
                _bookHeaders[GetBookKey(book)].Add(key, se);
            }
        }

        public void SetSort(States.SortState sortState)
        {
            if (SortState == sortState) return;

            SortState = sortState;

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

            UpdateActive();
        }

        public void SetShowUncastable(States.ShowUncastableState showUncastableState)
        {
            if (ShowUncastableState == showUncastableState) return;

            ShowUncastableState = showUncastableState;

            foreach (var e in _spellElements.Values)
                e.ShowIfUnavailable = ShowUncastableState == States.ShowUncastableState.Shown;

            UpdateActive();
        }



        public static string GetKey(Spellbook book, AbilityData spell) => $"{book.Blueprint.name}-{spell.SpellLevel}-{spell.SpellLevelInSpellbook}-{spell.Blueprint.name}";
        public static string GetBookKey(Spellbook book) => $"{book.Blueprint.Name}-Spellbook-Header";

    }
}
