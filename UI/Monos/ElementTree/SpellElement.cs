using Kingmaker.UnitLogic.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace QuickCast.UI.Monos.ElementTree
{
    internal class SpellElement : Element
    {

        public AbilityData Spell
        {
            get
            {
                return _spellData;
            }
            set
            {
                if(_spellTextMesh == null)
                {
                    _spellTextMesh = gameObject.GetComponentInChildren<TextMeshProUGUI>();
                }

                if(_spellTextMesh != null) 
                {
                    _spellTextMesh.text = value.Blueprint.Name;
                }
            }
        }
        public int Level;

        private TextMeshProUGUI _spellTextMesh;
        private AbilityData _spellData;

        public override void Initialize() { }
    }
}
