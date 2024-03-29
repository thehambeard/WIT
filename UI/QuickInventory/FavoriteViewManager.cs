﻿using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using ModMaker;
using ModMaker.Utility;
using QuickCast.Utilities;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace QuickCast.UI.QuickInventory
{
    public class FavoriteViewManager : ViewManager, IModEventHandler, ISelectionHandler, IViewChangeHandler
    {
        public int Priority => 500;

        public static FavoriteViewManager CreateObject(UnitEntityData unit)
        {
            var scrollview = GameObject.Instantiate(Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViewTemplate"), Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "ScrollViews"), false);
            scrollview.name = $"ScrollViewFavorite{unit.CharacterName}";
            scrollview.gameObject.SetActive(true);
            var scrollViewMono = scrollview.gameObject.AddComponent<FavoriteViewManager>();
            scrollViewMono._unit = unit;
            scrollViewMono._viewPortType = MainWindowManager.ViewPortType.Favorite;
            return scrollViewMono;
        }

        public override void Start()
        {
            base.Start();
            BuildFavoriteHeaders();
            //BuildList();
            //RestoreHeaders();
            //foreach (var button in transform.GetComponentsInChildren<Button>().Where(x => x.name == "SpellLevelBackground"))
            //{
            //    button.onClick.AddListener(() => HandleLevelClick(button));
            //}

            EventBus.Subscribe(this);
            gameObject.SetActive(false);
        }

        private void BuildList()
        {
            //foreach(var spells in Spells)
            //{
            //    if (!Entries.ContainsKey(spells.Key))
            //        Entries.Add(spells.Key, spells.Value);
            //}
        }

        private void BuildFavoriteHeaders()
        {
            _levelTransforms = new List<Transform>();
            _levelContentTransforms = new List<Transform>();

            var spellLevels = transform.Find("Viewport/Content/SpellLevel");
            var spellLevelsContent = transform.Find("Viewport/Content/SpellLevelContent");

            spellLevels.gameObject.SetActive(false);
            spellLevelsContent.gameObject.SetActive(false);

            //bool createStates = false;

            //if (SetWrap.HeaderStates == null)
            //    SetWrap.HeaderStates = new SerializableDictionary<MainWindowManager.ViewPortType, List<bool>>();

            //if (!SetWrap.HeaderStates.ContainsKey(_viewPortType))
            //{
            //    SetWrap.HeaderStates.Add(_viewPortType, new List<bool>());
            //    createStates = true;
            //}

            //if (createStates)
            //    SetWrap.HeaderStates[_viewPortType].Add(false);
            //var t = GameObject.Instantiate(spellLevels, spellLevels.parent, false);
            //var tc = GameObject.Instantiate(spellLevelsContent, spellLevelsContent.parent, false);
            //tc.name = "FavoriteSpellsContent";
            //tc.gameObject.SetActive(false);
            //tc.Find("Spell").SafeDestroy();
            //t.name = "FavoriteSpells";
            //t.GetComponentInChildren<TextMeshProUGUI>().text = "Spells";
            //t.gameObject.SetActive(false);
            //_levelTransforms.Add(t);
            //_levelContentTransforms.Add(tc);

            //spellLevels.SafeDestroy();
            //spellLevelsContent.SafeDestroy();
        }

        public void HandleModDisable()
        {
        }

        public void HandleModEnable()
        {
        }

        public void HandleViewChange() => this.OnUnitSelectionAdd(Game.Instance.UI.SelectionManager.SelectedUnits.FirstOrDefault<UnitEntityData>());

        public void OnUnitSelectionAdd(UnitEntityData selected)
        {
            if (Game.Instance.UI.SelectionManager.SelectedUnits.Count() != 1)
            {
                _multiSelected.gameObject.SetActive(true);
                _multiSelected.SetAsLastSibling();
                return;
            }
            else if (Main.Mod.Core.UI.MainWindowManager.CurrentViewPort == _viewPortType)
            {
                gameObject.SetActive(true);
                transform.SetAsLastSibling();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void OnUnitSelectionRemove(UnitEntityData selected)
        {
        }
    }
}
