using Kingmaker.UI.UnitSettings;
using Kingmaker.UnitLogic.Abilities;
using QuickCast.UI.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RootMotion.FinalIK.HitReactionVRIK;

namespace QuickCast.UI.Monos.ElementTree
{
    internal class SpellElement : Element
    {
        public int Level;
        public AbilityData Spell;

        private TextMeshProUGUI _spellTextMesh;
        private TextMeshProUGUI _usesTextMesh;

        private ButtonWrapper _actionButton;
        private ButtonWrapper _subElementButton;

        private RectTransform _uses;
        private RectTransform _spellText;
        private RectTransform _actions;
        private RectTransform _background;

        private float _usesMin;
        private float _spellTextMin;
        private float _backgroundMin;
        private float _actionsMin;

        private Vector2 _usesDelta;

        public override void Initialize() 
        {
            _uses = transform.Find("Uses").GetComponent<RectTransform>();
            _spellText = transform.Find("SpellText").GetComponent<RectTransform>();
            _actions = transform.Find("Actions").GetComponent<RectTransform>();
            _background = transform.Find("Background").GetComponent<RectTransform>();

            _spellTextMesh = _spellText.GetComponentInChildren<TextMeshProUGUI>();
            _usesTextMesh = _uses.GetComponentInChildren<TextMeshProUGUI>();

            _actionButton = _actions.Find("CastSpell").gameObject.AddComponent<ButtonWrapper>();
            _actionButton.Initialize();
            _actionButton.OnLeftClickEvent.AddListener(OnClickActionButton);

            var spellIcon = _uses.GetComponentInChildren<Image>();
            spellIcon.sprite = Spell.Icon;

            _spellTextMesh.text = Spell.Blueprint.Name;
            _usesTextMesh.text = Spell.GetAvailableForCastCount().ToString();

            _usesMin = _uses.offsetMin.x;
            _usesDelta = _uses.sizeDelta;
            
            _spellTextMin = _spellText.offsetMin.x;
            _backgroundMin = _background.offsetMin.x;
            _actionsMin = _actions.offsetMin.x;
        }

        public void OnClickActionButton()
        {
            if (Spell.IsSpontaneous)
            {
                var slot = new MechanicActionBarSlotSpontaneousSpell(Spell);
                slot.Unit = Unit;
                slot.OnClick();
            }
            else
            {
                var slot = new MechanicActionBarSlotMemorizedSpell(Spell.SpellSlot);
                slot.Unit = Unit;
                slot.OnClick();
            }
        }

        public override void SetElementLayout()
        {
            SetMin(_uses, _usesMin);
            SetMin(_spellText, _spellTextMin);
            SetMin(_background, _backgroundMin);
            SetMin(_actions, _actionsMin);
           
            SetMax(_uses, _usesDelta);
        }

        private void ShowSubElements()
        {

        }
    }
}
