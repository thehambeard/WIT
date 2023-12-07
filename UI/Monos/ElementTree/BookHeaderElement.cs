using Kingmaker.UnitLogic;
using System.Collections.Generic;

namespace QuickCast.UI.Monos.ElementTree
{
    internal class BookHeaderElement : HeaderElement
    {
        public Spellbook Spellbook;

        private List<SpellLevelHeaderElement> _levelHeaders = new();

        public void AddHeader(string key, SpellLevelHeaderElement element)
        {
            base.Add(key, element);
            _levelHeaders.Add(element);
        }

        public override void Add(string key, Element element)
        {
            if (element is SpellElement spell)
            {
                if (spell.Level <= 10)
                {
                    _levelHeaders[spell.Level].Add(key, spell);
                }
            }
        }

        public override bool UpdateActive()
        {
            foreach (var lh in _levelHeaders)
                lh.UpdateActive();

            return base.UpdateActive();
        }
    }
}
