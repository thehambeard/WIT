using Kingmaker.UI.UnitSettings;
using Kingmaker.UnitLogic.Abilities;
using QuickCast.UI.Utility;
using QuickCast.Utility.Extentions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuickCast.UI.Monos.ElementTree
{
    internal class SpellElement : Element
    {
        public int Level;
        public AbilityData Spell;
        public List<SpellElement> ConvertedSpells;
        public MechanicActionBarSlotSpell Slot;
        public bool Converted;

        private bool _showIfUnavailable = true;

        private TextMeshProUGUI _spellTextMesh;
        private TextMeshProUGUI _usesTextMesh;

        private QCButton _actionButton;
        private QCButton _subElementButton;

        private RectTransform _uses;
        private RectTransform _spellText;
        private RectTransform _actions;
        private RectTransform _background;

        private float _usesMin;
        private float _spellTextMin;
        private float _backgroundMin;
        private float _actionsMin;

        private Vector2 _usesDelta;

        public bool ShowIfUnavailable
        {
            get { return _showIfUnavailable; }
            set
            {
                _showIfUnavailable = value;
                UpdateActive();
            }
        }

        public bool IsHiddenUnavailable => !Spell.IsAvailableForCast && !ShowIfUnavailable;

        public override void Initialize()
        {
            _uses = transform.Find("Uses").GetComponent<RectTransform>();
            _spellText = transform.Find("SpellText").GetComponent<RectTransform>();
            _actions = transform.Find("Actions").GetComponent<RectTransform>();
            _background = transform.Find("Background").GetComponent<RectTransform>();

            _spellTextMesh = _spellText.GetComponentInChildren<TextMeshProUGUI>();
            _usesTextMesh = _uses.GetComponentInChildren<TextMeshProUGUI>();

            _actionButton = Builders.BuildUI.BuildQCButton<QCButton>(
                button: _actions.Find("CastSpell"),
                toggable: false,
                onLeftClick: OnClickActionButton);

            var spellIcon = _uses.GetComponentInChildren<Image>();
            spellIcon.sprite = Spell.Icon;

            _spellTextMesh.text = Spell.Blueprint.Name;
            _usesTextMesh.text = Spell.GetAvailableForCastCount().ToString();

            _usesMin = _uses.offsetMin.x;
            _usesDelta = _uses.sizeDelta;

            _spellTextMin = _spellText.offsetMin.x;
            _backgroundMin = _background.offsetMin.x;
            _actionsMin = _actions.offsetMin.x;

            IsExpanded = false;

            InitializeSpellSlot();
        }

        private void InitializeSpellSlot()
        {
            ConvertedSpells = new();

            if (Spell.IsSpontaneous || Spell.SpellLevel == 0 || Converted)
            {
                Slot = new MechanicActionBarSlotSpontaneousSpell(Spell)
                {
                    Unit = Unit
                };
            }
            else
            {
                Slot = new MechanicActionBarSlotMemorizedSpell(Spell.SpellSlot)
                {
                    Unit = Unit
                };
            }

            foreach (var convert in Spell.GetConversions())
            {
                var key = $"{this.gameObject.name}-{convert.Name}";
                var element = Builders.BuildUI.BuildSpellElement(this.transform.parent, convert, Unit, convert.SpellLevel, key, true);
                element.gameObject.SetActive(false);
                ConvertedSpells.Add(element);
                this.Add(key, element);
            }
        }

        public void OnDestroy()
        {
            foreach (var convert in ConvertedSpells)
                convert.SafeDestroy();
        }

        public void OnClickActionButton()
        {
            if (Slot.IsNotAvailable)
            {
                ToggleExpanded();
                return;
            }

            Slot.OnClick();
        }

        private void ToggleExpanded()
        {
            IsExpanded = !IsExpanded;

            foreach (var convert in ConvertedSpells)
                convert.UpdateActive();
        }

        public override void SetElementLayout()
        {
            SetMin(_uses, _usesMin);
            SetMin(_spellText, _spellTextMin);
            SetMin(_background, _backgroundMin);
            SetMin(_actions, _actionsMin);

            SetMax(_uses, _usesDelta);
        }

        public override bool UpdateActive()
        {
            bool show = !IsHidden && (Parent == null ? true : (Parent.IsExpanded && Parent.gameObject.activeSelf)) && !IsHiddenUnavailable;

            if (gameObject.activeSelf != show)
                gameObject.SetActive(show);

            return show;
        }
    }
}
