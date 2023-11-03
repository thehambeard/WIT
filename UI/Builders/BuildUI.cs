using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Owlcat.Runtime.Core.Utils;
using QuickCast.UI.Monos;
using QuickCast.UI.Monos.Controls;
using QuickCast.UI.Monos.ViewControlGroup;
using QuickCast.UI.Monos.ViewControlGroup.SpellSV;
using QuickCast.UI.Utility;
using UnityEngine;

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

        private static void BuildWindowControls(Transform parent)
        {
            var wcg = Create(Prefabs.WindowControlGroup, parent);

            wcg.transform.FindChildRecursive("Move").gameObject.AddComponent<DraggableWindow>(); ;

            var resizeTop = wcg.transform.FindChildRecursive("ResizeTop").gameObject.AddComponent<QCResizePanel>();
            resizeTop.Pivot = QCResizePanel.ResizePivot.Top;

            var resizeLeft = wcg.transform.FindChildRecursive("ResizeLeft").gameObject.AddComponent<QCResizePanel>();
            resizeLeft.Pivot = QCResizePanel.ResizePivot.Left;

            var resizeRight = wcg.transform.FindChildRecursive("ResizeRight").gameObject.AddComponent<QCResizePanel>();
            resizeRight.Pivot = QCResizePanel.ResizePivot.Right;

            var resizeBottom = wcg.transform.FindChildRecursive("ResizeBottom").gameObject.AddComponent<QCResizePanel>();
            resizeBottom.Pivot = QCResizePanel.ResizePivot.Bottom;

            wcg.SetActive(true);
        }

        public static SpellSVManager BuildSpellScrollView(Transform parent, UnitEntityData unit)
        {
            var svm = Create(Prefabs.ScrollView, parent).AddComponent<SpellSVManager>();
            
            svm.gameObject.name = $"{unit.CharacterName}-SpellScrollView";
            svm.Unit = unit;

            return svm;
        }

        public static SpellLevelHeaderElement BuildLevelHeaderElement(Transform parent, int level)
        {
            var lhe = Create(Prefabs.LevelHeaderElement,parent).AddComponent<SpellLevelHeaderElement>();

            lhe.gameObject.name = $"LevelHeaderElement-{level}";
            lhe.Level = level;

            return lhe;
        }

        public static BookHeaderElement BuildBookHeaderElement(Transform parent, Spellbook book)
        {
            var bhe = Create(Prefabs.LevelHeaderElement, parent).AddComponent<BookHeaderElement>();

            bhe.gameObject.name = $"BookHeaderElement-{book.Blueprint.name}";
            bhe.Spellbook = book;

            return bhe;
        }

        public static SpellElement BuildSpellElement(Transform parent, AbilityData spell)
        {
            var se = Create(Prefabs.SpellElement, parent).AddComponent<SpellElement>();

            se.Initialize(spell);
            
            return se;
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
