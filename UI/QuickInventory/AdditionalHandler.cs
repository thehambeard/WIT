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
        }

        public void Show(RectTransform link, EntryData data, UnitEntityData unit)
        {
            _createdTransforms = new List<Transform>();

            LinkedTransform = link;
            EntryData = data;
            Unit = unit;

            if (link == null || data == null)
                return;

            var rectTransform = (RectTransform)transform;
            _addAbilities = data.MSlot.GetConvertedAbilityData();

            if (_addAbilities == null)
                return;

            Transform trans;

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, _addAbilities.Count * 24.05f);

            foreach (var a in _addAbilities)
            {
                trans = GameObject.Instantiate(_template, _template.parent, false);
                trans.gameObject.SetActive(true);
                trans.name = a.Name;
                trans.GetComponentInChildren<TextMeshProUGUI>().text = a.Name;
                var button = trans.GetComponentInChildren<Button>();
                button.onClick.AddListener(() => OnClick(button));
                _createdTransforms.Add(trans);
            }
            rectTransform.position = new Vector3(rectTransform.position.x, LinkedTransform.position.y, rectTransform.position.z);
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y + 13.3f, rectTransform.localPosition.z);
            gameObject.SetActive(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }

        public void Update()
        {
            if(Input.GetMouseButtonDown(0) && !_mouseIsOver)
            {
                Hide();
            }
        }
        
        public void Hide()
        {
            foreach(var t in _createdTransforms)
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
