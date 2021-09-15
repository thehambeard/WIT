using Kingmaker.UI.Selection;
using ModMaker.Utility;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WIT.UI.QuickInventory
{
    public class ViewManager : MonoBehaviour
    {
        protected List<Dictionary<object, ItemButtonManager>> _ViewContent = new List<Dictionary<object, ItemButtonManager>>();
        protected List<string> _HeaderTitles;
        protected RectTransform _LevelTemplate;
        protected RectTransform _LevelTemplateParent;
        private ItemButtonManager _ButtonTemplate;

        public virtual void Start()
        {
            if (!_LevelTemplate)
            {
                _LevelTemplate = (RectTransform)transform.Find("Viewport/Content/Level0/");
                _LevelTemplateParent = (RectTransform)_LevelTemplate.parent;
                _LevelTemplate.SetParent(null, false);
                _LevelTemplate.gameObject.SetActive(false);
                DontDestroyOnLoad(_LevelTemplate);
            }

            BuildView();
        }

        public virtual void Update()
        {
            UpdateView();
        }

        public void HandleHeaderClick(RectTransform rect)
        {
            var shrink = rect.Find("ShrinkExpand/Shrink").gameObject;
            var expand = rect.Find("ShrinkExpand/Expand").gameObject;
            var content = rect.parent.Find("Content").gameObject;

            shrink.SetActive(!shrink.activeSelf);
            expand.SetActive(!expand.activeSelf);
            content.SetActive(!content.activeSelf);
        }

        protected void RemoveButton(ItemButtonManager button)
        {
            foreach (var v in _ViewContent)
                v.Remove(button.Data);
            button.SafeDestroy();
        }

        protected ItemButtonManager Ensure(object data, int level, int index, ref bool isDirty)
        {
            if (!_ViewContent[level].TryGetValue(data, out ItemButtonManager button))
            {
                if (!_ButtonTemplate)
                {
                    _ButtonTemplate = ItemButtonManager.CreateObject(_LevelTemplate.Find("Content/Item").gameObject);
                    _ButtonTemplate.gameObject.SetActive(false);
                    DontDestroyOnLoad(_ButtonTemplate.gameObject);
                }
                button = GameObject.Instantiate(_ButtonTemplate);
                button.Data = data;

                _ViewContent[level].Add(data, button);
                isDirty = true;
            }
            else if (button.Index != index)
            {
                button.Index = index;
                isDirty = true;
            }

            return button;
        }

        private void UpdateView()
        {
            RectTransform rect, parent;
            bool empty = true;
            var selection = SelectionManager.Instance;

            if (_HeaderTitles == null || selection == null) return;

            if (selection.SelectedUnits.Count != 1)
            {
                for (int i = 0; i < _HeaderTitles.Count - 1; i++)
                {
                    rect = (RectTransform)_LevelTemplateParent.GetChild(i);
                    rect.gameObject.SetActive(false);
                }
                rect = (RectTransform)_LevelTemplateParent.GetChild(_HeaderTitles.Count - 1);
                rect.gameObject.SetActive(true);
                return;
            }

            rect = (RectTransform)_LevelTemplateParent.GetChild(_HeaderTitles.Count - 1);
            rect.gameObject.SetActive(false);

            for (int i = 0; i < _HeaderTitles.Count - 1; i++)
            {
                empty = empty && _ViewContent[i].Count <= 0;
                rect = (RectTransform)_LevelTemplateParent.GetChild(i);
                rect.gameObject.SetActive(_ViewContent[i].Count > 0);
                parent = (RectTransform)rect.Find("Content");
                foreach (KeyValuePair<object, ItemButtonManager> ite_to_add in _ViewContent[i])
                {
                    ite_to_add.Value.transform.SetParent(parent, false);
                    ite_to_add.Value.gameObject.SetActive(true);
                }
            }
            rect = (RectTransform)_LevelTemplateParent.GetChild(_HeaderTitles.Count - 2);
            rect.gameObject.SetActive(empty);
        }

        private void BuildView()
        {
            RectTransform entry;

            for (int i = 0; i < _HeaderTitles.Count; i++)
            {
                entry = GameObject.Instantiate(_LevelTemplate);
                entry.SetParent(_LevelTemplateParent, false);
                entry.gameObject.name = $"Level{i}";
                entry.gameObject.SetActive(_ViewContent[i].Count > 0);

                var level = entry.Find("Header");
                level.gameObject.SetActive(true);
                var tmp = level.GetComponentInChildren<TextMeshProUGUI>();
                level.GetComponent<Button>().onClick.AddListener(() => { HandleHeaderClick((RectTransform)level); });
                tmp.text = _HeaderTitles[i];
                var ite_template = GameObject.Instantiate(entry.Find("Content/Item"));
                var content_parent = entry.Find("Content");
                DestroyImmediate(entry.Find("Content/Item").gameObject);
                foreach (KeyValuePair<object, ItemButtonManager> ite_to_add in _ViewContent[i])
                {
                    ite_to_add.Value.transform.SetParent(content_parent, false);
                    ite_to_add.Value.gameObject.SetActive(true);
                }
            }
        }
    }
}