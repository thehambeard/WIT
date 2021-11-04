using DG.Tweening;
using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI.UnitSettings;
using ModMaker.Utility;
using QuickCast.Utilities;
using System;
using System.Collections;
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
        protected Transform _multiSelected;
        protected Transform _noSpells;
        protected UnitEntityData _unit;
        protected MainWindowManager.ViewPortType _viewPortType;
        protected List<Transform> _levelTransforms;
        protected List<Transform> _levelContentTransforms;

        private Transform _template;
        private Transform _spellTemplate;
        private Transform _additional;

        protected void RemoveTransform(string key, Dictionary<string, EntryData> entries, Transform parentTransform, Transform sibTransform)
        {
            if (entries.ContainsKey(key))
            {
                GameObject.DestroyImmediate(entries[key].Transform.gameObject);
                entries.Remove(key);
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

        public virtual void Start()
        {
            Entries = new Dictionary<string, EntryData>();
            _levelTransforms = new List<Transform>();
            _levelContentTransforms = new List<Transform>();

            _template = Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate");
            _spellTemplate = _template.Find("Viewport/Content/SpellLevelContent/Spell");
            _multiSelected = transform.parent.FirstOrDefault(x => x.name == "MultiSelected");
            _noSpells = transform.parent.FirstOrDefault(x => x.name == "NoSpells");
            _additional = transform.parent.parent.FirstOrDefault(x => x.name == "Additional");
        }

        protected void BuildHeaders(ref List<Transform> levelHeaderContent, ref List<Transform> levelHeaders)
        {
            var spellLevels = transform.Find("Viewport/Content/SpellLevel");
            var spellLevelsContent = transform.Find("Viewport/Content/SpellLevelContent");
            bool createStates = false;

            if (SetWrap.HeaderStates == null)
            {
                SetWrap.HeaderStates = new SerializableDictionary<MainWindowManager.ViewPortType, List<bool>>();
            }

            if (!SetWrap.HeaderStates.ContainsKey(_viewPortType))
            {
                SetWrap.HeaderStates.Add(_viewPortType, new List<bool>());
                createStates = true;
            }

            for (int i = 0; i <= 10; i++)
            {
                if (createStates)
                    SetWrap.HeaderStates[_viewPortType].Add(false);
                var t = GameObject.Instantiate(spellLevels, spellLevels.parent, false);
                var tc = GameObject.Instantiate(spellLevelsContent, spellLevelsContent.parent, false);
                tc.name = $"SpellLevelContent{i}";
                tc.gameObject.SetActive(false);
                tc.Find("Spell").SafeDestroy();
                t.name = $"SpellLevel{i}";
                t.GetComponentInChildren<TextMeshProUGUI>().text = $"Level {i}";
                var button = t.GetComponentInChildren<Button>();
                button.onClick.AddListener(() => HandleLevelClick(button));
                t.gameObject.SetActive(false);
                tc.gameObject.SetActive(false);
                levelHeaders.Add(t);
                levelHeaderContent.Add(tc);
            }

            spellLevels.SafeDestroy();
            spellLevelsContent.SafeDestroy();
        }

        protected EntryData InsertTransform(MechanicActionBarSlot mslot, string entryName, Transform parentTransform, Transform sibTransform)
        {
            var spellContentTransform = GameObject.Instantiate(_spellTemplate, parentTransform, false);
            var text = spellContentTransform.GetChild(1).GetComponent<TextMeshProUGUI>();
            var additional = spellContentTransform.GetChild(2).GetComponent<Button>();
            additional.gameObject.SetActive(mslot.GetConvertedAbilityData().Any() || _viewPortType == MainWindowManager.ViewPortType.Potions);
            spellContentTransform.name = entryName;
            text.text = entryName;
            text.color = new Color(.31f, .31f, .31f);
            var button = spellContentTransform.GetComponentInChildren<Button>();
            var entry = new EntryData()
            {
                EntryText = entryName,
                MSlot = mslot,
                Transform = spellContentTransform,
                Button = button,
                DCText = spellContentTransform.FirstOrDefault(x => x.name == "DCText").GetComponent<TextMeshProUGUI>(),
                UsesText = spellContentTransform.FirstOrDefault(x => x.name == "UsesText").GetComponent<TextMeshProUGUI>()

            };
            button.onClick.AddListener(() => RunCommand(entry));
            if (_viewPortType == MainWindowManager.ViewPortType.Potions)
                additional.onClick.AddListener(() => RunCommandOtherPotion(entry));
            else
                additional.onClick.AddListener(() => ShowAdditional(entry, additional));
            parentTransform.gameObject.SetActive(true);
            sibTransform.gameObject.SetActive(true);
            return entry;
        }

        protected void RestoreHeaders()
        {
            if (!SetWrap.HeaderStates.ContainsKey(_viewPortType))
                return;

            for (int i = 0; i < SetWrap.HeaderStates[_viewPortType].Count; i++)
            {
                if(_levelTransforms[i].gameObject.activeSelf)
                    HandleLevelClick(_levelTransforms[i].GetComponentInChildren<Button>(), SetWrap.HeaderStates[_viewPortType][i] ? 2 : 1);
            }
        }

        public void ToggleCollapseExpandAll()
        {
            bool noneActive = true;

            foreach (Transform t in _levelContentTransforms)
            {
                if (t.gameObject.activeSelf)
                {
                    noneActive = false;
                    break;
                }
            }

            foreach (var button in transform.GetComponentsInChildren<Button>().Where(x => x.name == "SpellLevelBackground"))
            {
                HandleLevelClick(button, (noneActive) ? 2 : 1);
            }
        }

        protected void HandleLevelClick(Button button, int forceState = 0)
        {
            var toggleExpand = button.transform.parent.GetChild(2);
            var index = button.transform.parent.GetSiblingIndex() / 2;
            var content = _levelContentTransforms[index];
            var alpha = content.GetComponent<CanvasGroup>();

            var state = !SetWrap.HeaderStates[_viewPortType][index];
            switch (forceState)
            {
                case 1:
                    state = false;
                    break;
                case 2:
                    state = true;
                    break;

            }
            SetWrap.HeaderStates[_viewPortType][index] = state;
            if (state)
            {
                toggleExpand.DORotate(new Vector3(0f, 0f, 0f), .25f).SetUpdate(true);
                alpha.DOFade(1f, .5f).SetUpdate(true);
            }
            else
            {
                toggleExpand.DORotate(new Vector3(0, 0, 180f), .25f).SetUpdate(true);
                alpha.DOFade(0f, .1f).SetUpdate(true);
            }
            content.gameObject.SetActive(state);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) content.parent);
        }

        private void RunCommandOtherPotion(EntryData entry)
        {
            var item = (MechanicActionBarSlotItem)entry.MSlot;
            if (Game.Instance.UI.SelectionManager.SelectedUnits.Count != 1)
                return;
            var actionBarSlotAbility = new MechanicActionBarSlotAbility();
            var wielder = Game.Instance.UI.SelectionManager.SelectedUnits.FirstOrDefault<UnitEntityData>();
            var quickSlot = wielder.Body.QuickSlots[wielder.Body.QuickSlots.Length - 1];
            if (quickSlot.HasItem) quickSlot.RemoveItem();
            quickSlot.InsertItem(item.Item);

            var ability = item.GetConvertedAbilityData().FirstOrDefault() ;
            if (ability == null)
                return;
            
            actionBarSlotAbility.Ability = ability;
            actionBarSlotAbility.Unit = wielder;
            actionBarSlotAbility.OnClick();
        }

        private void ShowAdditional(EntryData entry, Button button)
        {
            var adhandle = _additional.GetComponent<AdditionalHandler>();
            adhandle.Show((RectTransform)button.transform, entry, _unit);
        }
        
        private void RunCommand(EntryData entry)
        { 
            switch(entry.MSlot)
            {
                case MechanicActionBarSlotSpontaneousSpell spontaneousSpell:
                    spontaneousSpell.OnClick();
                    break;
                case MechanicActionBarSlotMemorizedSpell memorizedSpell:
                    memorizedSpell.OnClick();
                    break;
                case MechanicActionBarSlotItem item:
                    if (Game.Instance.UI.SelectionManager.SelectedUnits.Count != 1)
                        return;
                    var actionBarSlotAbility = new MechanicActionBarSlotAbility();
                    var wielder = Game.Instance.UI.SelectionManager.SelectedUnits.FirstOrDefault<UnitEntityData>();
                    var quickSlot = wielder.Body.QuickSlots[wielder.Body.QuickSlots.Length - 1];
                    if (quickSlot.HasItem) quickSlot.RemoveItem();
                    quickSlot.InsertItem(item.Item);
                    //item was not working with some items, this hack fixed the problem. Not ideal.
                    actionBarSlotAbility.Ability = quickSlot.Item.Ability.Data;
                    actionBarSlotAbility.Unit = wielder;
                    actionBarSlotAbility.OnClick();
                    break;
                case MechanicActionBarSlotAbility ability:
                    ability.OnClick();
                    break;
                case MechanicActionBarSlotActivableAbility activableAbility:
                    activableAbility.OnClick();
                    break;
            }
        }

        protected void SortTransforms()
        {
            int i = 0;
            foreach (var t in Entries.OrderBy(x => x.Value.EntryText))
                t.Value.Transform.SetSiblingIndex(i++);
        }

        protected void SortTransforms(Dictionary<string, EntryData> entries)
        {
            int i = 0;
            foreach (var t in entries.OrderBy(x => x.Value.EntryText))
                t.Value.Transform.SetSiblingIndex(i++);
        }

    }
}
