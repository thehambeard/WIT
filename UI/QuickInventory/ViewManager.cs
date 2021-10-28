using DG.Tweening;
using Kingmaker;
using Kingmaker.UI.UnitSettings;
using ModMaker.Utility;
using QuickCast.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static QuickCast.Main;

namespace QuickCast.UI.QuickInventory
{
    public class ViewManager : MonoBehaviour
    {
        protected Dictionary<string, EntryData> Entries;

        private Transform _template;
        private Transform _spellTemplate;
        private bool _expandAll = true;

        protected void RemoveTransform(string key, MechanicActionBarSlot ability, Transform parentTransform, Transform sibTransform)
        {
            if (Entries.ContainsKey(key))
            {
                GameObject.DestroyImmediate(Entries[key].Transform.gameObject);
                Entries.Remove(key);
            }
            if (parentTransform == null || sibTransform == null)
                return;

            if (parentTransform.childCount <= 0)
            {
                parentTransform.gameObject.SetActive(false);
                sibTransform.gameObject.SetActive(false);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parentTransform);
        }

        public virtual void Awake()
        {
            _template = Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate");
            _spellTemplate = _template.Find("Viewport/Content/SpellLevelContent/Spell");


        }

        protected virtual void BuildHeaders(ref List<Transform> contentTransforms, ref List<Transform> levelTransforms)
        {
            var spellLevels = transform.Find("Viewport/Content/SpellLevel");
            var spellLevelsContent = transform.Find("Viewport/Content/SpellLevelContent");

            for (int i = 0; i <= 10; i++)
            {
                var t = GameObject.Instantiate(spellLevels, spellLevels.parent, false);
                var tc = GameObject.Instantiate(spellLevelsContent, spellLevelsContent.parent, false);
                tc.name = $"SpellLevelContent{i}";
                tc.gameObject.SetActive(false);
                tc.Find("Spell").SafeDestroy();
                t.name = $"SpellLevel{i}";
                t.GetComponentInChildren<TextMeshProUGUI>().text = $"Level {i} Spells";
                var button = t.GetComponentInChildren<Button>();
                button.onClick.AddListener(() => HandleLevelClick(button));
                t.gameObject.SetActive(false);
                levelTransforms.Add(t);
                contentTransforms.Add(tc);
            }

            spellLevels.SafeDestroy();
            spellLevelsContent.SafeDestroy();
        }

        protected EntryData InsertTransform(MechanicActionBarSlotSpell ability, Transform parentTransform, Transform sibTransform)
        {
            var spellContentTransform = GameObject.Instantiate(_spellTemplate, parentTransform, false);
            var text = spellContentTransform.GetChild(1).GetComponent<TextMeshProUGUI>();
            spellContentTransform.name = ability.Spell.Name;
            text.text = ability.Spell.Name;
            text.color = new Color(.31f, .31f, .31f);
            var button = spellContentTransform.GetComponentInChildren<Button>();
            var entry = new EntryData()
            {
                EntryText = name,
                MSlot = ability,
                Transform = spellContentTransform,
                Button = button,
                DCText = spellContentTransform.FirstOrDefault(x => x.name == "DCText").GetComponent<TextMeshProUGUI>(),
                UsesText = spellContentTransform.FirstOrDefault(x => x.name == "UsesText").GetComponent<TextMeshProUGUI>()

            };
            button.onClick.AddListener(() => RunCommand(entry));
            parentTransform.gameObject.SetActive(true);
            sibTransform.gameObject.SetActive(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parentTransform);

            return entry;
        }

        public void ToggleCollapseExpandAll()
        {
            foreach (var button in transform.GetComponentsInChildren<Button>().Where(x => x.name == "SpellLevelBackground"))
            {
                HandleLevelClick(button, (_expandAll) ? 1 : 2);
            }
            _expandAll = !_expandAll;
        }

        protected void HandleLevelClick(Button button, int forceState = 0)
        {
            var toggleExpand = button.transform.parent.GetChild(2);
            bool active;
            switch (forceState)
            {
                case 1:
                    active = true;
                    break;
                case 2:
                    active = false;
                    break;
                default:
                    active = !button.transform.parent.parent.GetChild(button.transform.parent.GetSiblingIndex() + 1).gameObject.activeSelf;
                    break;
            }
            button.transform.parent.parent.GetChild(button.transform.parent.GetSiblingIndex() + 1).gameObject.SetActive(active);
            if (active)
                toggleExpand.DORotate(new Vector3(0f, 0f, 0f), .25f).SetUpdate(true);
            else
                toggleExpand.DORotate(new Vector3(0, 0, 180f), .25f).SetUpdate(true);
        }

        private void RunCommand(EntryData entry)
        {
            switch(entry.MSlot)
            {
                case MechanicActionBarSlotSpontaneousSpell spontaneousSpell:
                    foreach (var v in spontaneousSpell.GetConvertedAbilityData())
                        Mod.Debug(v.Name);
                    spontaneousSpell.OnClick();
                    break;
                case MechanicActionBarSlotMemorizedSpell memorizedSpell:
                    memorizedSpell.OnClick();
                    break;
            }
        }

        protected void SortTransforms()
        {
            int i = 0;
            foreach (var t in Entries.OrderBy(x => x.Value.EntryText))
                t.Value.Transform.SetSiblingIndex(i++);
        }

    }
}
