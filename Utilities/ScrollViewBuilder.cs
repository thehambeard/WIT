using Kingmaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Kingmaker.UnitLogic;
using Kingmaker.EntitySystem.Entities;
using static WIT.Main;
using Kingmaker.UnitLogic.Abilities;
using WIT.UI.QuickInventory;
using UnityEngine.UI;

namespace WIT.Utilities
{
    public class ScrollViewBuilder
    {
        public RectTransform ScrollViewTemplate;

        public ScrollViewBuilder()
        {
            var transform = Game.Instance.UI.Canvas.transform.Find("QuickInventory").FirstOrDefault(x => x.name == "ScrollViewTemplate");
            ScrollViewTemplate = (RectTransform)GameObject.Instantiate(transform);
        }

        public Transform TransBookEntry(Spellbook book)
        {
            var transform = GameObject.Instantiate(ScrollViewTemplate.FirstOrDefault(x => x.name == "Content"));
            var transformBook = transform.Find("SpellBook");
            var transformContent = transform.Find("SpellBookContent");
            foreach (Transform t in transformContent)
                GameObject.Destroy(t.gameObject);
            transform.GetComponentInChildren<TextMeshProUGUI>().text = book.Blueprint.DisplayName;
            transform.name = transform.name.Replace("(Clone)", book.Blueprint.DisplayName);
            transformContent.name += book.Blueprint.DisplayName;
            transformBook.name += book.Blueprint.DisplayName;
            transform.gameObject.SetActive(true);
            return transform;
        }

        public Transform TransLevelEntry(int i, Spellbook spellbook)
        {
            var transform = GameObject.Instantiate(ScrollViewTemplate.FirstOrDefault(x => x.name == "SpellBookContent"));
            var transformSpell = transform.FirstOrDefault(x => x.name == "SpellLevel");
            var transformContent = transform.FirstOrDefault(x => x.name == "SpellLevelContent");
            
            foreach (Transform t in transformContent)
                GameObject.Destroy(t.gameObject);
            transform.GetComponentInChildren<TextMeshProUGUI>().text = $"Level {i} Spells";
            transform.name = transform.name.Replace("(Clone)", "");
            transformContent.name += $"{spellbook.Blueprint.DisplayName}Level{i}";
            transformSpell.name += $"{spellbook.Blueprint.DisplayName}Level{i}";
            transform.gameObject.SetActive(true);
            return transform;
        }

        public Transform TransSpellEntry(AbilityData spell, int count)
        {
            var transform = GameObject.Instantiate(ScrollViewTemplate.FirstOrDefault(x => x.name == "Spell"));
            var tmp = transform.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = spell.Name;
            tmp.color = new Color(.31f, .24f, .31f);
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            transform.name = transform.name.Replace("(Clone)", "") + $": {spell}";
            transform.FirstOrDefault(x => x.name == "UsesText").GetComponent<TextMeshProUGUI>().text = count.ToString();
            transform.gameObject.SetActive(true);
            return transform;
        }

        public Transform TransSpellEntry(SpellSlot spell)
        {
            var transform = GameObject.Instantiate(ScrollViewTemplate.FirstOrDefault(x => x.name == "Spell"));
            var tmp = transform.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = spell.Spell.Name;
            tmp.color = new Color(.31f, .24f, .31f);
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            transform.name = transform.name.Replace("(Clone)", "") + $": {spell.Spell}";
            transform.FirstOrDefault(x => x.name == "UsesText").GetComponent<TextMeshProUGUI>().text = spell.BusySlotsCount.ToString();
            transform.gameObject.SetActive(true);
            return transform;
        }

        public Transform ReturnEmpty(string name)
        {
            var transfrom = GameObject.Instantiate(ScrollViewTemplate);
            transfrom.name = name;
            foreach (Transform t in transfrom.FirstOrDefault(x => x.name == "Content"))
                GameObject.Destroy(t.gameObject);
            return transfrom;
        }
    }
}
