using DG.Tweening;
using Kingmaker;
using Kingmaker.Localization;
using Kingmaker.UI.Common;
using Kingmaker.UI.Constructor;
using Kingmaker.UI.Tooltip;
using Kingmaker.View;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using WIT.Utilities;
using static WIT.Main;

namespace WIT.UI.QuickInventory
{ 
    public class MainWindowManager : MonoBehaviour
    {
        private readonly static string _source = "QuickCanvas";
        private List<Button> _selectBarButtons;

        public static MainWindowManager CreateObject()
        {
            var staticCanvas = Game.Instance.UI.Canvas;
            var fadeCanvas = Game.Instance.UI.FadeCanvas;

            //return null if no game canvas is avail.
            if (staticCanvas == null || fadeCanvas == null) return null;

            try
            {
                //get wrath object that are needed. Scrollbar and TMPro material
                var wrathScrollBar = staticCanvas.transform?.Find("HUDLayout/CombatLog_New/Scroll View/ScrollbarVertical/") ?? throw new NullReferenceException("wrathScrollBal");
                var wrathTMPro = staticCanvas.transform?.Find("HUDLayout/CombatLog_New/TooglePanel/ToogleAll/ToogleAll/")?.GetComponent<TextMeshProUGUI>() ?? throw new NullReferenceException("wrathTMProMat");

                //instantiate the main window.  Assets from the loaded asset bundle are persistent 
                var mainWindow = (RectTransform) GameObject.Instantiate(AssetBundleManager.GameObjects[_source]).transform.Find("QuickInventory");
                mainWindow.SetParent(staticCanvas.transform, false);
                mainWindow.name = "QuickInventory";

                //trash my ugly scroll bar and attach new hotness to all the ScrollViews
                
                var scrollViews = mainWindow?.Find("QuickWindow/ScrollViews/") ?? throw new NullReferenceException("scrollViews");
                RectTransform newScrollBar;
                
                var numScrollChilden = scrollViews.childCount;
                for (int i = 0; i < numScrollChilden; i++)
                {
                    var scrollView = scrollViews.GetChild(i);
                    Mod.Debug(scrollView.GetChild(1).name);
                    DestroyImmediate(scrollView.GetChild(1).gameObject);
                    newScrollBar = (RectTransform) GameObject.Instantiate(wrathScrollBar);
                    newScrollBar.SetParent(scrollView, false);
                    newScrollBar.localScale = new Vector2(1.8f, 0.97f);
                    newScrollBar.localPosition = new Vector2(177.3f, 1.5f);
                    newScrollBar.Find("Back").GetComponent<Image>().color = new Color(.9f, .9f, .9f);

                    var scrollRectExtended = scrollView.gameObject.AddComponent<ScrollRectExtended>();
                    scrollRectExtended.viewport = (RectTransform) scrollView.GetChild(0);
                    scrollRectExtended.content = (RectTransform) scrollView.GetChild(0).GetChild(0);
                    scrollRectExtended.movementType = ScrollRectExtended.MovementType.Clamped;
                    scrollRectExtended.scrollSensitivity = 35f;
                    scrollRectExtended.verticalScrollbar = newScrollBar.GetComponent<Scrollbar>();

                    scrollView.Find("Viewport/Content/Header/Title").GetComponent<TextMeshProUGUI>().AssignFontApperanceProperties(wrathTMPro); 
                    scrollView.Find("Viewport/Content/AbilityEntry/Ability").GetComponent<TextMeshProUGUI>().AssignFontApperanceProperties(wrathTMPro);
                    scrollView.Find("Viewport/Content/AbilityEntry/Uses/Count").GetComponent<TextMeshProUGUI>().AssignFontApperanceProperties(wrathTMPro);
                }



                //Set up our buttons
                _ = mainWindow?.Find("QuickWindow/SelectBar")?.GetComponentsInChildren<TextMeshProUGUI>()?.AssignAllFontApperanceProperties(wrathTMPro) ?? throw new NullReferenceException("scrollViews");

                mainWindow.localPosition = new Vector3(0f, 0f, 0f);
                mainWindow.localScale = SetWrap.Window_Scale != null ? SetWrap.Window_Scale : new Vector3(.9f, .9f, .9f);
                
                var pos = SetWrap.Window_Pos !=null ? SetWrap.Window_Pos :new Vector3(Screen.width * .5f, Screen.height * .5f, Camera.main.WorldToScreenPoint(staticCanvas.transform.Find("HUDLayout").position).z);

                mainWindow.gameObject.SetActive(true);

                //Return instance to the controller
                //Add as component to the mainWindow transform, unity will automatically send messages for Update method
                return mainWindow.gameObject.AddComponent<MainWindowManager>();
            }
            catch (NullReferenceException ex)
            {
                Mod.Debug($"{ex.Message} has returned null. Stacktrace: {ex.StackTrace}");
            }

            return null;
        }

        void Awake()
        {

        }

        void Update()
        {

        }


    }
}
