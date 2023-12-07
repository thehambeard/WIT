using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using System.Collections.Generic;

namespace QuickCast.Utility.Extentions
{
    internal static class OwlcatExtentions
    {
        public static IEnumerable<AbilityData> GetAllSpellsFromBook(this Spellbook book)
        {
            List<AbilityData> spells = new();

            spells.AddRange(book.GetAllKnownSpells());

            for (int i = 0; i < book.MaxSpellLevel; i++)
                spells.AddRange(book.GetSpecialSpells(i));

            return spells;
        }
    }
}
