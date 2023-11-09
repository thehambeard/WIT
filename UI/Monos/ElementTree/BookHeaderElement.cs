using Kingmaker.UnitLogic;
using QuickCast.UI.Builders;
using QuickCast.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickCast.UI.Monos.ElementTree
{
    internal class BookHeaderElement : HeaderElement
    {
        public Spellbook Spellbook;

        private List<LevelHeaderElement> _levelHeaders = new();

        public void AddHeader(string key, LevelHeaderElement element)
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
    }
}
