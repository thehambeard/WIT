using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace QuickCast.UI.Monos.ViewControlGroup.SpellSV
{
    internal class SpellSVManager : MonoBehaviour
    {
        public UnitEntityData Unit;
        public List<SpellLevelHeaderElement> SpellLevelHeaderElements;
        public Dictionary<Spellbook, BookHeaderElement> BookHeaderElements;
        public Dictionary<string, SpellElement> SpellElements;

        private ScrollRect _scrollRect;
        private States.SortState _sortState;

        public void Initialize()
        {
            _scrollRect = GetComponent<ScrollRect>();

            SpellLevelHeaderElements = new();
            BookHeaderElements = new();
            SpellElements = new();

            for (int i = 0; i <= 10; i++)
                SpellLevelHeaderElements.Add(Builders.BuildUI.BuildLevelHeaderElement(_scrollRect.content, i));

            foreach(var book in Unit.Spellbooks) 
            {
                if (!BookHeaderElements.ContainsKey(book))
                    BookHeaderElements.Add(book, Builders.BuildUI.BuildBookHeaderElement(_scrollRect.content, book));

                foreach(var spell in book.GetAllKnownSpells())
                {
                    if(!SpellElements.ContainsKey(GetKey(book, spell)))
                    {
                        int level = spell.SpellLevelInSpellbook ?? spell.SpellLevel;

                        if (level <= 10)
                        {
                            AddSpellElement(spell, level, book);
                        }
                    }
                }
            }
            SetSort(States.SortState.Level);
        }

        public void SetSort(States.SortState sortState)
        {
            if (_sortState == sortState) return;

            _sortState = sortState;

            switch (sortState) 
            {
                case States.SortState.Level:
                    SortBy(SpellLevelHeaderElements);
                    foreach(var e in BookHeaderElements.Values)
                        e.Unclaim();
                    break;
                case States.SortState.Book:
                    SortBy(BookHeaderElements.Values.ToList());
                    foreach (var e in SpellLevelHeaderElements)
                        e.Unclaim();
                    break;
            }
        }

        public void SortBy<T>(List<T> headers) where T : HeaderElement
        {
            foreach (var header in headers)
                header.Claim();
        }

        public void AddSpellElement(AbilityData spell, int level, Spellbook book)
        {
            var se = Builders.BuildUI.BuildSpellElement(_scrollRect.content, spell);

            if (!SpellElements.ContainsKey(GetKey(book, spell)))
            {
                SpellElements.Add(GetKey(book, spell), se);
                SpellLevelHeaderElements[level].Add(se);
                BookHeaderElements[book].Add(se);
            }
        }

        public string GetKey(Spellbook book, AbilityData spell) => $"{book.Blueprint.name}-{spell.SpellLevel}-{spell.SpellLevelInSpellbook}-{spell.Blueprint.name}";

    }
}
