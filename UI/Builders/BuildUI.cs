using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Owlcat.Runtime.Core.Utils;
using QuickCast.UI.Monos;
using QuickCast.UI.Monos.Controls;
using QuickCast.UI.Monos.ElementTree;
using QuickCast.UI.Monos.ViewControlGroup;
using QuickCast.UI.Monos.ViewControlGroup.MetaMagic;
using QuickCast.UI.Monos.ViewControlGroup.ScrollViewMode;
using QuickCast.UI.Monos.ViewControlGroup.SpellSV;
using QuickCast.UI.Utility;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace QuickCast.UI.Builders
{
    internal class BuildUI
    {
        public static MainUIManager BuildMainUI()
        {
            var staticCanvas = Game.Instance.UI.Canvas;

            var main = Create(Prefabs.Main, staticCanvas.gameObject);
            main.SetActive(true);
            var mainUIManager = main.AddComponent<MainUIManager>();

            BuildWindowControls(main.transform);

            var vcg = Create(Prefabs.ViewControlGroup, main).AddComponent<VCGManager>();
            vcg.gameObject.SetActive(true);
            vcg.Initialize();

            return mainUIManager;
        }

        public static SVModeManager BuildScrollViewMode(VCGManager parent)
        {
            var svm = Create(Prefabs.ScrollViewMode, parent.gameObject).AddComponent<SVModeManager>();
            svm.gameObject.SetActive(true);
            svm.Initialize(parent);
            return svm;
        }

        public static MetaMagicCtrlManager BuildMetaMagicCtrl(VCGManager parent)
        {
            var mmc = Create(Prefabs.MetaMagicCtrl, parent.gameObject).AddComponent<MetaMagicCtrlManager>();
            mmc.gameObject.SetActive(true);
            mmc.Initialize(parent);
            return mmc;
        }

        public static T BuildQCButton<T>(Transform button, bool toggable, UnityAction onLeftClick = null, Sprite defaultSprite = null, string defaultText = "", string toggleText = "") where T : QCButton
        {
            var oldButton = button.GetComponent<Button>();

            if (oldButton == null)
                throw new NullReferenceException();

            var image = button.GetComponent<Image>();

            var spriteState = oldButton.spriteState;
            var colorBlock = oldButton.colors;
            var transition = oldButton.transition;
            GameObject.DestroyImmediate(oldButton);

            var newButton = button.gameObject.AddComponent<T>();
            newButton.spriteState = spriteState;
            newButton.colors = colorBlock;
            newButton.transition = transition;
            newButton.IsToggable = toggable;
            newButton.DefaultSprite = defaultSprite == null ? image.sprite : defaultSprite;

            newButton.DefaultText = defaultText;
            newButton.ToggleText = toggleText;

            newButton.onClick = new Button.ButtonClickedEvent();

            if (onLeftClick != null)
                newButton.onClick.AddListener(onLeftClick);

            newButton.Initialize();

            return newButton;
        }

        public static MetaButton BuildMetaButton(Transform parent, MetaMagicCtrlManager mmcManger)
        {
            var mb = BuildQCButton<MetaButton>(
                button: Create(Prefabs.MetaButton, parent).transform,
                toggable: true);

            mb.gameObject.SetActive(false);

            mb.Initialize(mmcManger);

            return mb;
        }


        private static void BuildWindowControls(Transform parent)
        {
            var wcg = Create(Prefabs.WindowControlGroup, parent);

            wcg.transform.FindChildRecursive("Move").gameObject.AddComponent<QCDraggableWindow>(); ;

            var resizeTop = wcg.transform.FindChildRecursive("ResizeTop").gameObject.AddComponent<QCResizePanel>();
            resizeTop.Pivot = QCResizePanel.ResizePivot.Top;

            var resizeLeft = wcg.transform.FindChildRecursive("ResizeLeft").gameObject.AddComponent<QCResizePanel>();
            resizeLeft.Pivot = QCResizePanel.ResizePivot.Left;

            var resizeRight = wcg.transform.FindChildRecursive("ResizeRight").gameObject.AddComponent<QCResizePanel>();
            resizeRight.Pivot = QCResizePanel.ResizePivot.Right;

            var resizeBottom = wcg.transform.FindChildRecursive("ResizeBottom").gameObject.AddComponent<QCResizePanel>();
            resizeBottom.Pivot = QCResizePanel.ResizePivot.Bottom;

            AddScalableWindowComponent(wcg.transform.Find("UpperControlGroup/LeftMinScale/ScaleCtrl"), QCScalableWindow.WindowCorner.TopLeft);
            AddScalableWindowComponent(wcg.transform.Find("UpperControlGroup/RightMinScale/ScaleCtrl"), QCScalableWindow.WindowCorner.TopRight);
            AddScalableWindowComponent(wcg.transform.Find("LowerControlGroup/LeftMinScale/ScaleCtrl"), QCScalableWindow.WindowCorner.BottomLeft);
            AddScalableWindowComponent(wcg.transform.Find("LowerControlGroup/RightMinScale/ScaleCtrl"), QCScalableWindow.WindowCorner.BottomRight);

            wcg.SetActive(true);
        }

        private static void AddScalableWindowComponent(Transform target, QCScalableWindow.WindowCorner corner)
        {
            foreach (Transform t in target)
            {
                var comp = t.gameObject.AddComponent<QCScalableWindow>();
                comp.ScaleCorner = corner;
            }
        }

        public static SpellSVManager BuildSpellScrollView(Transform parent, UnitEntityData unit)
        {
            var svm = Create(Prefabs.ScrollView, parent).AddComponent<SpellSVManager>();

            svm.gameObject.name = $"{unit.CharacterName}-SpellScrollView";
            svm.Unit = unit;

            return svm;
        }

        public static SpellElement BuildSpellElement(Transform parent, AbilityData spell, UnitEntityData unit, int Level, string key, bool converted = false)
        {
            var se = Create(Prefabs.SpellElement, parent).AddComponent<SpellElement>();

            se.gameObject.name = key;
            se.Level = Level;
            se.Spell = spell;
            se.Converted = converted;
            se.Unit = unit;
            se.AllowUnclaim = false;
            se.ShowIfChildless = true;
            se.ShowIfUnclaimed = true;

            se.Initialize();

            return se;
        }

        public static SpellLevelHeaderElement BuildSpellLevelHeaderElement(Transform parent, int level, string key = "", string suffix = "")
        {
            var lhe = Create(Prefabs.LevelHeaderElement, parent).AddComponent<SpellLevelHeaderElement>();

            lhe.Level = level;
            BuildHeaderElement(lhe, $"{key}{level:00}-Header", $"Level {level} {suffix}");

            return lhe;
        }

        public static BookHeaderElement BuildBookHeaderElement(Transform parent, Spellbook book)
        {
            var bhe = Create(Prefabs.BookHeaderElement, parent).AddComponent<BookHeaderElement>();

            bhe.Spellbook = book;
            BuildHeaderElement(bhe, $"{book.Blueprint.Name}-Spellbook-Header", book.Blueprint.Name);

            for (int i = 0; i <= 10; i++)
            {
                var lhe = BuildSpellLevelHeaderElement(parent, i, book.Blueprint.Name, "Spells");
                bhe.AddHeader(lhe.gameObject.name, lhe);
            }

            return bhe;
        }

        public static void BuildHeaderElement<T>(T headerElement, string name, string title) where T : HeaderElement
        {
            headerElement.gameObject.name = name;
            headerElement.ShowIfChildless = false;
            headerElement.AllowUnclaim = true;
            headerElement.ShowIfUnclaimed = false;
            headerElement.Title = title;
            headerElement.Initialize();
        }

        private static GameObject Create(GameObject obj, GameObject parent)
        {
            return Create(obj, parent.transform);
        }

        private static GameObject Create(GameObject obj, Transform parent)
        {
            var o = GameObject.Instantiate(obj, parent, false);
            o.name = obj.name;
            return o;
        }
    }
}
