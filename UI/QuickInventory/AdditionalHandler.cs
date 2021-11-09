using Kingmaker;
using Kingmaker.UnitLogic.Abilities;
using QuickCast.UI.QuickInventory;
using QuickCast.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Kingmaker.UI.UnitSettings;
using static QuickCast.Main;
using Kingmaker.EntitySystem.Entities;

namespace QuickCast.UI.QuickInventory
{
    public class AdditionalHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public EntryData EntryData;
        public RectTransform LinkedTransform;
        public UnitEntityData Unit;

        private Transform _template;
        private Dictionary<string, RectTransform> _transforms;
        private List<Transform> _createdTransforms;
        private bool _mouseIsOver = false;
        private List<AbilityData> _addAbilities;
        private Vector2 _leftPos;
        private Vector2 _rightPos;
        protected void Awake()
        {
            if (_transforms == null)
            {
                _transforms = transform.GetComponentsInChildren<RectTransform>().ToDictionary(key => key.name, value => value);
            }
            if (!_template)
            {
                _template = _transforms["Spell"];
                _template.GetComponentInChildren<TextMeshProUGUI>().color = new Color(.31f, .31f, .31f);
                _template.gameObject.SetActive(false);
            }

            _rightPos = transform.localPosition;
            _leftPos = new Vector2(transform.localPosition.x - ((RectTransform) transform).rect.width - ((RectTransform)transform.parent).rect.width, transform.localPosition.y - 3f);
        }

        public void Show(RectTransform link, EntryData data, UnitEntityData unit)
        {
            var rect = (RectTransform)transform;
            _createdTransforms = new List<Transform>();

            LinkedTransform = link;
            EntryData = data;
            Unit = unit;

            if (link == null || data == null)
                return;

            for(int i = _template.parent.childCount; i > 1; i--)
                GameObject.DestroyImmediate(_template.parent.GetChild(i - 1).gameObject);

            _addAbilities = data.MSlot.GetConvertedAbilityData();

            if (_addAbilities == null)
                return;

            Transform trans;

            if (Input.mousePosition.x > Screen.width * .65f)
            {
                rect.localPosition = _leftPos;
            }
            else
            {
                rect.localPosition = _rightPos;
            }

            foreach (var a in _addAbilities.OrderBy(x => x.Name))
            {
                trans = GameObject.Instantiate(_template, _template.parent, false);
                trans.gameObject.SetActive(true);
                trans.name = a.Name;
                trans.GetChild(1).GetComponent<TextMeshProUGUI>().text = a.Name;
                trans.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = a.GetAvailableForCastCount().ToString();
                var button = trans.GetComponentInChildren<Button>();
                button.onClick.AddListener(() => OnClick(button));
                _createdTransforms.Add(trans);
            }
            gameObject.SetActive(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }

        public void Update()
        {
            if (gameObject.activeSelf && Input.GetMouseButtonDown(0) && !_mouseIsOver)
            {
                Hide();
            }
        }
        
        public void Hide()
        {
            if (!gameObject.activeSelf)
                return;

            foreach (var t in _createdTransforms)
                GameObject.DestroyImmediate(t.gameObject);

            gameObject.SetActive(false);

            _createdTransforms = null;
            _addAbilities = null;

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }

        public void OnClick(Button button)
        {
            var index = button.transform.parent.GetSiblingIndex() - 1;
            MechanicActionBarSlotAbility ability = new MechanicActionBarSlotAbility() { Ability = _addAbilities[index], Unit = this.Unit};
            ability.OnClick();
            Hide();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseIsOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseIsOver = false;
        }
    }
}
