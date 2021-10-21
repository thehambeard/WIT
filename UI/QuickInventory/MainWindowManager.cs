﻿using DG.Tweening;
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
using Kingmaker.UI;
using Owlcat.Runtime.Core.Logging;
using Kingmaker.PubSubSystem;

namespace WIT.UI.QuickInventory
{ 
    public class MainWindowManager : MonoBehaviour
    {
        private static readonly string _source = "QuickCanvas";
        
        //for changing and track whichever viewport is being shown.
        private List<ViewButtonWrapper> _viewButtons;
        public  ViewPortType CurrentViewPort = ViewPortType.Spells;
        private static StaticCanvas _staticCanvas;
        private static FadeCanvas _fadeCanvas;
        private Vector3 _minMaxPos;
        private Button _minWin;
        private Button _scaleWin;
        private Button _settingsWin;
        private RectTransform _minRect;
        private Button _collapseExpandWin;
        private List<Button> _moveButton;

        public enum ViewPortType
        {
            Spells,
            Scrolls,
            Potions,
            Wands,
            Special,
            Favorite
        }

        public static MainWindowManager CreateObject()
        {
            _staticCanvas = Game.Instance.UI.Canvas;
            _fadeCanvas = Game.Instance.UI.FadeCanvas;

            //return null if no game canvas is avail.
            if (_staticCanvas == null || _fadeCanvas == null) return null;

            try
            {
                //get wrath object that are needed. Scrollbar and TMPro material
                var wrathScrollBar = _staticCanvas.transform?.Find("HUDLayout/CombatLog_New/Panel/Scroll View/ScrollbarVertical/") ?? throw new NullReferenceException("wrathScrollBal");
                var wrathTMPro = _staticCanvas.transform?.Find("HUDLayout/CombatLog_New/TooglePanel/ToogleAll/ToogleAll/")?.GetComponent<TextMeshProUGUI>() ?? throw new NullReferenceException("wrathTMProMat");

                //instantiate the main window.  Assets from the loaded asset bundle are persistent 
                var mainWindow = (RectTransform) GameObject.Instantiate(AssetBundleManager.GameObjects[_source]).transform.Find("QuickInventory");
                mainWindow.SetParent(_staticCanvas.transform, false);
                mainWindow.name = "QuickInventory";

                //trash my ugly scroll bar and attach new hotness to all the ScrollViews

                var scrollView = mainWindow?.FirstOrDefault(x => x.name == "ScrollViewTemplate") ?? throw new NullReferenceException("scrollView");
                RectTransform newScrollBar;
                GameObject.DestroyImmediate(scrollView.FirstOrDefault(x => x.name == "ScrollbarVerticle").gameObject);

                newScrollBar = (RectTransform) GameObject.Instantiate(wrathScrollBar);
                newScrollBar.SetParent(scrollView, false);
                newScrollBar.localScale = new Vector2(1.8f, 0.97f);
                newScrollBar.localPosition = new Vector2(-5f, 1.5f);
                newScrollBar.GetComponentInChildren<Scrollbar>().direction = Scrollbar.Direction.BottomToTop;
                newScrollBar.Find("Back").GetComponent<Image>().color = new Color(.9f, .9f, .9f);

                var scrollRectExtended = scrollView.gameObject.AddComponent<ScrollRectExtended>();
                scrollRectExtended.viewport = (RectTransform) scrollView.GetChild(0);
                scrollRectExtended.content = (RectTransform) scrollView.GetChild(0).GetChild(0);
                scrollRectExtended.movementType = ScrollRectExtended.MovementType.Clamped;
                scrollRectExtended.scrollSensitivity = 35f;
                scrollRectExtended.verticalScrollbar = newScrollBar.GetComponent<Scrollbar>();

                mainWindow.GetComponentsInChildren<TextMeshProUGUI>().AssignAllFontApperanceProperties(wrathTMPro);
                mainWindow.FirstOrDefault(x => x.name == "NoSpells").GetComponentInChildren<TextMeshProUGUI>().AssignFontApperanceProperties(wrathTMPro);
                mainWindow.FirstOrDefault(x => x.name == "MultiSelected").GetComponentInChildren<TextMeshProUGUI>().AssignFontApperanceProperties(wrathTMPro);
                mainWindow.pivot = new Vector2(1f, 0f);
                mainWindow.localPosition = SetWrap.Window_Pos;
                mainWindow.localScale = SetWrap.Window_Scale;
                mainWindow.gameObject.SetActive(true);
                mainWindow.SetAsFirstSibling();

                mainWindow.FirstOrDefault(x => x.name == "ScrollViewTemplate").gameObject.SetActive(false);

                return mainWindow.gameObject.AddComponent<MainWindowManager>();
            }
            catch (NullReferenceException ex)
            {
                Mod.Error($"{ex.Message} has returned null. Stacktrace: {ex.StackTrace}");
            }

            return null;
        }

        void Awake()
        {
            int index = 0;
            _viewButtons = new List<ViewButtonWrapper>();
            foreach (var button in transform.Find("QuickWindow/SelectBar").GetComponentsInChildren<Button>())
                _viewButtons.Add(new ViewButtonWrapper(this, button, (ViewPortType) index++));

            if (_moveButton == null)
                _moveButton = new List<Button>();
            _minWin = transform.Find("QuickWindow/WindowButtons/MinWindowButton").GetComponent<Button>();
            _collapseExpandWin = transform.Find("QuickWindow/WindowButtons/ToggleTreeButton").GetComponent<Button>();
            _scaleWin = transform.Find("QuickWindow/WindowButtons/ScaleWindowButton").GetComponent<Button>();
            _settingsWin = transform.Find("QuickWindow/WindowButtons/SettingsWindowButton").GetComponent<Button>();
            _moveButton.Add(transform.Find("QuickWindow/WindowButtons/MoveWindowButton1").GetComponent<Button>());
            _moveButton.Add(transform.Find("QuickWindow/WindowButtons/MoveWindowButton2").GetComponent<Button>());
            _moveButton.Add(transform.Find("QuickWindow/WindowButtons/MoveWindowButton3").GetComponent<Button>());
            _moveButton.Add(transform.Find("QuickWindow/WindowButtons/MoveWindowButton4").GetComponent<Button>());
            foreach(var m in _moveButton)
                m.gameObject.AddComponent<DraggableWindow>();

            _minRect = (RectTransform)transform.Find("Min_Window");
            _minRect.gameObject.SetActive(false);
            _minRect.GetComponent<CanvasGroup>().alpha = 0f;
            _minRect.anchoredPosition = new Vector2(-60f, 60f);
            _minRect.gameObject.AddComponent<DraggableWindow>();
            var mindWindowButton = _minRect.GetComponent<Button>();
            mindWindowButton.onClick = new Button.ButtonClickedEvent();
            mindWindowButton.onClick.AddListener(new UnityAction(HandleMaximizeOnClick));


            _scaleWin.gameObject.AddComponent<ScalableWindow>();

            new WindowButtonWrapper(_minWin, HandleMinimizeOnClick, "Minimize", "Minimizes the window.");
            new WindowButtonWrapper(_collapseExpandWin, HandleCollapseExpand, "Expand / Collapse", "Click to toggle collapse all or expand all");
            new WindowButtonWrapper(_scaleWin, HandleScaleOnClick, "Scale Window", "Click and drag to scale the window.");
            new WindowButtonWrapper(_settingsWin, HandleSettingsOnClick, "Settings", "Opens the settings window.");
            foreach(var move in _moveButton)
                new WindowButtonWrapper(move, HandleMoveDrag, "Move", "Click and Drag to move the window");

            _viewButtons.FirstOrDefault().IsPressed = true;
        }
        void Update()
        {

        }

        private void HandleCollapseExpand()
        {

        }

        private void HandleMaximizeOnClick()
        {
            StartCoroutine(MaxWindow());
        }
        private void HandleMinimizeOnClick()
        {
            StartCoroutine(MinWindow());
        }
        private void HandleMoveDrag()
        {
            SetWrap.Window_Pos = transform.localPosition;
            SetWrap.Window_Scale = transform.localScale;
        }

        private void HandleScaleOnClick()
        {

        }

        private void HandleSettingsOnClick()
        {
        }

        private IEnumerator MaxWindow()
        {
            var windowRect = transform.Find("QuickWindow");
            _minRect.GetComponent<CanvasGroup>().alpha = 0f;
            var tween = windowRect.DOScale(1f, .25f).SetUpdate(true);
            windowRect.DOLocalMove(_minMaxPos, .25f).SetUpdate(true);
            windowRect.gameObject.SetActive(true);
            
            yield return tween.WaitForCompletion();
            _minRect.gameObject.SetActive(false);
        }

        private IEnumerator MinWindow()
        {
            var windowRect = transform.Find("QuickWindow");
            _minMaxPos = windowRect.localPosition;
            var tween = windowRect.DOScale(.1f, .25f).SetUpdate(true);
            windowRect.DOLocalMove(_minRect.localPosition, .25f).SetUpdate(true);
            _minRect.gameObject.SetActive(true);
            _minRect.GetComponent<CanvasGroup>().DOFade(1f, .25f);
            yield return tween.WaitForCompletion();
            windowRect.gameObject.SetActive(false);
        }

        
        private void HandleViewButtonClick(ViewPortType index)
        {
            if (index == CurrentViewPort) return;
            foreach (ViewButtonWrapper b in _viewButtons) b.IsPressed = false;
            _viewButtons[(int) index].IsPressed = true;
            CurrentViewPort = index;
            if (Game.Instance.UI.SelectionManager.SelectedUnits.Count == 1)
            {
                EventBus.RaiseEvent<IViewChangeHandler>((Action<IViewChangeHandler>)(h => h.HandleViewChange(index)));
            }
        }

        private class WindowButtonWrapper
        {
            public WindowButtonWrapper(Button button, Action action, string title, string description)
            {
                var _tooltip = button.gameObject.AddComponent<TooltipTrigger>();
                _tooltip.enabled = true;
                _tooltip.SetNameAndDescription(title, description);
                button.gameObject.AddComponent<OnHover>();
                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(new UnityAction(action));
            }
        }

        internal class OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
        {
            public bool IsHover { get; private set; }
            public bool IsPressed { get; private set; }

            private void Awake()
            {
                gameObject.GetComponent<CanvasGroup>().alpha = 0f;
                IsHover = false;
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                gameObject.GetComponent<CanvasGroup>().DOFade(1f, .25f);
                IsHover = true;
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                gameObject.GetComponent<CanvasGroup>().DOFade(0f, .25f);
                IsHover = false;
            }

            public void OnPointerUp(PointerEventData eventData)
            {
                IsPressed = true;
            }

            public void OnPointerDown(PointerEventData eventData)
            {
                IsPressed = false;
            }
        }
        
        private class ViewButtonWrapper
        {
            private bool _isPressed;

            private readonly Button _button;
            private readonly Image _image;
            public SpriteState _pressed;
            public SpriteState _default;
            public Sprite _defaultSprite;
            public Sprite _pressedSprite;

            public bool IsPressed
            {
                get => _isPressed;
                set
                {
                    if (_isPressed != value)
                    {
                        _isPressed = value;
                        if (value)
                        {
                            _button.spriteState = _pressed;
                            _image.sprite = _pressed.pressedSprite;
                        }
                        else
                        {
                            _button.spriteState = _default;
                            _image.sprite = _defaultSprite;
                        }
                    }
                }
            }

            public ViewButtonWrapper(MainWindowManager ui, Button button, ViewPortType index)
            {
                _button = button;
                _button.onClick = new Button.ButtonClickedEvent();
                _button.onClick.AddListener(() => ui.HandleViewButtonClick(index));
                _image = _button.gameObject.GetComponent<Image>();
                _defaultSprite = _image.sprite;
                _pressedSprite = _button.spriteState.pressedSprite;
                _default = _button.spriteState;
                _pressed = _default;
                _pressed.disabledSprite = _pressed.pressedSprite;
                _pressed.highlightedSprite = _pressed.pressedSprite;
            }
        }
    }
}
