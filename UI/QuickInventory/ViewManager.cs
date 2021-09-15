using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using static WIT.Main;
using Kingmaker;
using WIT.Utilities;
using Kingmaker.UI;
using TMPro;
using Kingmaker.UI.Common;
using UnityEngine.UI;
using Kingmaker.UI.Constructor;
using UnityEngine.Events;
using Kingmaker.Localization;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic;
using DG.Tweening;
using ModMaker.Utility;
using Kingmaker.UI.Group;
using UnityEngine.EventSystems;
using Kingmaker.UI.Selection;

namespace WIT.UI.QuickInventory
{
    public class ViewManager : MonoBehaviour
    {
        protected List<Dictionary<object, ItemButtonManager>> m_ViewContent = new List<Dictionary<object, ItemButtonManager>>();
        protected List<string> m_HeaderTitles;
        protected RectTransform m_LevelTemplate;
        protected RectTransform m_LevelTemplateParent;
        private ItemButtonManager m_ButtonTemplate;

        public virtual void Start()
        {
            if (!m_LevelTemplate)
            {
                m_LevelTemplate = (RectTransform)transform.Find("Viewport/Content/Level0/");
                m_LevelTemplateParent = (RectTransform)m_LevelTemplate.parent;
                m_LevelTemplate.SetParent(null, false);
                m_LevelTemplate.gameObject.SetActive(false);
                DontDestroyOnLoad(m_LevelTemplate);
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
            foreach (var v in m_ViewContent)
                v.Remove(button.Data);
            button.SafeDestroy();
        }

        protected ItemButtonManager Ensure(object data, int level, int index, ref bool isDirty)
        {
            if (!m_ViewContent[level].TryGetValue(data, out ItemButtonManager button))
            {
                if (!m_ButtonTemplate)
                {
                    m_ButtonTemplate = ItemButtonManager.CreateObject(m_LevelTemplate.Find("Content/Item").gameObject);
                    m_ButtonTemplate.gameObject.SetActive(false);
                    DontDestroyOnLoad(m_ButtonTemplate.gameObject);
                }
                button = GameObject.Instantiate(m_ButtonTemplate);
                button.Data = data;

                m_ViewContent[level].Add(data, button);
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

            if (m_HeaderTitles == null || selection == null) return;

            if(selection.SelectedUnits.Count != 1)
            {
                for (int i = 0; i < m_HeaderTitles.Count - 1; i++)
                {
                    rect = (RectTransform)m_LevelTemplateParent.GetChild(i);
                    rect.gameObject.SetActive(false);
                }
                rect = (RectTransform)m_LevelTemplateParent.GetChild(m_HeaderTitles.Count - 1);
                rect.gameObject.SetActive(true);
                return;
            }

            rect = (RectTransform)m_LevelTemplateParent.GetChild(m_HeaderTitles.Count - 1);
            rect.gameObject.SetActive(false);

            for (int i = 0; i < m_HeaderTitles.Count - 1; i++)
            {
                empty = empty && m_ViewContent[i].Count <= 0;
                rect = (RectTransform) m_LevelTemplateParent.GetChild(i);
                rect.gameObject.SetActive(m_ViewContent[i].Count > 0);
                parent = (RectTransform)rect.Find("Content");
                foreach (KeyValuePair<object, ItemButtonManager> item_to_add in m_ViewContent[i])
                {
                    item_to_add.Value.transform.SetParent(parent, false);
                    item_to_add.Value.gameObject.SetActive(true);
                }
            }
            rect = (RectTransform)m_LevelTemplateParent.GetChild(m_HeaderTitles.Count - 2);
            rect.gameObject.SetActive(empty);
        }

        private void BuildView()
        {
            RectTransform entry;

            for (int i = 0; i < m_HeaderTitles.Count; i++)
            {
                entry = GameObject.Instantiate(m_LevelTemplate);
                entry.SetParent(m_LevelTemplateParent, false);
                entry.gameObject.name = $"Level{i}";
                entry.gameObject.SetActive(m_ViewContent[i].Count > 0);

                var level = entry.Find("Header");
                level.gameObject.SetActive(true);
                var tmp = level.GetComponentInChildren<TextMeshProUGUI>();
                level.GetComponent<Button>().onClick.AddListener(() => { HandleHeaderClick((RectTransform)level); });
                tmp.text = m_HeaderTitles[i];
                var item_template = GameObject.Instantiate(entry.Find("Content/Item"));
                var content_parent = entry.Find("Content");
                DestroyImmediate(entry.Find("Content/Item").gameObject);
                foreach (KeyValuePair<object, ItemButtonManager> item_to_add in m_ViewContent[i])
                {
                    item_to_add.Value.transform.SetParent(content_parent, false);
                    item_to_add.Value.gameObject.SetActive(true);
                }
            }
        }
    }
}
