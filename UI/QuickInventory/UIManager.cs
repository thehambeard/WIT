using DG.Tweening;
using Kingmaker;
using Kingmaker.Localization;
using Kingmaker.UI.Common;
using Kingmaker.UI.Constructor;
using Kingmaker.UI.Tooltip;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WIT.Utilities;
using static WIT.Main;

namespace WIT.UI.QuickInventory
{
    internal class UIManager : MonoBehaviour
    {
        private const string _source = "QuickCanvas";
        private const float _scale = .85f;
        private List<ViewButtonWrapper> _buttons;
        private List<RectTransform> _spellViews;
        private int _currentIndex;

        public event Action OnEnter;

        private RectTransform _ownRect;
        private Vector2 _position;
        private RectTransform _minWindow;

        public static RectTransform Revert;

        public static UIManager CreateObject()
        {
            try
            {
                if (Game.Instance.UI.Canvas == null) return null;
                var staticCanvas = Game.Instance.UI.Canvas.RectTransform ?? throw new NullReferenceException("staticCanvas");
                var fadeCanvas = Game.Instance.UI.FadeCanvas.transform ?? throw new NullReferenceException("fadeCanvas");
                var kmButtonTMP = fadeCanvas?.Find("EscMenuView/Window/ButtonBlock/SaveButton/Text/")?.gameObject ?? throw new NullReferenceException("kmButtonTMP");
                RectTransform kmHUDTMP = null; //staticCanvas?.Find("ServiceWindowsPCView/EncyclopediaView/EncyclopediaNavigationView/BodyGroup/StandardScrollView/Viewport/Content/EncyclopediaNavigationFirstView(Clone)/MultiButton/Label")?.gameObject ?? throw new NullReferenceException("kmHUDTMP");
                var kmToogle = staticCanvas?.Find("HUDLayout/CombatLog_New/TooglePanel/ToogleAll/")?.gameObject ?? throw new NullReferenceException("kmToogleButton");
                var kmScrollBar = staticCanvas?.Find("HUDLayout/CombatLog_New/Scroll View/ScrollbarVertical/")?.gameObject ?? throw new NullReferenceException("kmScrollbar");

                if (!BundleManger.IsLoaded(_source)) throw new Exception(_source);
                var instance = GameObject.Instantiate(BundleManger.LoadedPrefabs[_source]);
                var window = (RectTransform)instance?.transform?.Find("QuickInventory") ?? throw new NullReferenceException("window");
                window.SetParent(Game.Instance.UI.Common.transform, false);
                window.SetSiblingIndex(0);
                var quickWindow = (RectTransform)window.Find("QuickWindow") ?? throw new NullReferenceException("quickWindow");
                var spellView = (RectTransform)quickWindow?.Find("Scroll View") ?? throw new NullReferenceException("scrollView");
                var scrollBar = (RectTransform)spellView?.Find("Scrollbar Vertical") ?? throw new NullReferenceException("scrollBar");
                var viewport = (RectTransform)spellView?.Find("Viewport") ?? throw new NullReferenceException("viewport");
                var svcontent = (RectTransform)viewport?.Find("Content") ?? throw new NullReferenceException("svcontent");
                var selectBar = (RectTransform)quickWindow?.Find("SelectBar") ?? throw new NullReferenceException("selectBar");
                var buttonsTMP = (RectTransform)selectBar?.Find("BagButton") ?? throw new NullReferenceException("buttonBarTMP");
                var levelHeaderTMP = (RectTransform)svcontent?.Find("Level0/Header/") ?? throw new NullReferenceException("levelHeaderTMP");
                var itemTMP = (RectTransform)svcontent?.Find("Level0/Content/Item/") ?? throw new NullReferenceException("itemTMP");
                var minWindow = (RectTransform)window?.transform?.Find("Min_Window") ?? throw new NullReferenceException("minWindow");

                minWindow.gameObject.SetActive(false);
                minWindow.gameObject.GetComponent<CanvasGroup>().alpha = 0;

                var newScrollBar = GameObject.Instantiate(kmScrollBar).GetComponent<Scrollbar>();
                newScrollBar.transform.localPosition = new Vector3(-5f, 1.5f, 0f);
                newScrollBar.transform.SetParent(spellView, false);
                newScrollBar.transform.Find("Back").GetComponent<Image>().color = new Color(.9f, .9f, .9f);
                newScrollBar.transform.localScale = new Vector3(.7f, .987f, 1f);
                GameObject.DestroyImmediate(scrollBar.gameObject);
                var newSRE = spellView.gameObject.AddComponent<ScrollRectExtended>();
                newSRE.viewport = viewport;
                newSRE.content = svcontent;
                newSRE.movementType = ScrollRectExtended.MovementType.Clamped;
                newSRE.scrollSensitivity = 35f;
                newSRE.verticalScrollbar = newScrollBar;
                var newLevelObj = GameObject.Instantiate(kmButtonTMP, levelHeaderTMP, false);
                DestroyImmediate(newLevelObj.GetComponent<LocalizedUIText>());
                var newLevelTMP = newLevelObj.GetComponent<TextMeshProUGUI>();
                newLevelTMP.text = "Level 1";
                newLevelTMP.alignment = TextAlignmentOptions.MidlineLeft;
                newLevelTMP.fontSize = 27f;
                var newLevelLayout = newLevelObj.AddComponent<LayoutElement>();
                newLevelObj.transform.SetAsFirstSibling();
                newLevelLayout.flexibleWidth = 1f;
                newLevelLayout.flexibleHeight = 1f;

                var newItemTMP = GameObject.Instantiate(kmHUDTMP, itemTMP, false).GetComponent<TextMeshProUGUI>();
                itemTMP.gameObject.AddComponent<ButtonPF>();
                newItemTMP.text = "Magic Missle";
                newItemTMP.alignment = TextAlignmentOptions.MidlineLeft;
                newItemTMP.fontSize = 24f;
                newItemTMP.enableWordWrapping = false;
                newItemTMP.enableAutoSizing = true;
                newItemTMP.fontSizeMin = 12f;
                newItemTMP.fontSizeMax = 16f;
                newItemTMP.overflowMode = TextOverflowModes.Ellipsis;
                var newItemObj = itemTMP?.Find("Label(Clone)")?.gameObject ?? throw new NullReferenceException("newLevelObj");
                var newItemLayout = newItemObj.AddComponent<LayoutElement>();
                newItemLayout.flexibleWidth = 0f;
                newItemLayout.flexibleHeight = 1f;

                spellView.gameObject.name = "SpellView";
                var scrollView = GameObject.Instantiate(spellView.gameObject, quickWindow, false);
                var wandView = GameObject.Instantiate(spellView.gameObject, quickWindow, false);
                var potionView = GameObject.Instantiate(spellView.gameObject, quickWindow, false);
                var specialView = GameObject.Instantiate(spellView.gameObject, quickWindow, false);
                spellView.gameObject.AddComponent<SpellViewManager>();
                scrollView.AddComponent<ScrollItemViewManager>();
                wandView.AddComponent<WandItemViewManager>();
                potionView.AddComponent<PotionItemViewManager>();

                selectBar.SetAsLastSibling();
                GameObject.DestroyImmediate(buttonsTMP.gameObject.GetComponent<Button>());
                GameObject spellButton = GameObject.Instantiate(buttonsTMP.gameObject, selectBar, false);
                GameObject scrollButton = GameObject.Instantiate(buttonsTMP.gameObject, selectBar, false);
                GameObject wandButton = GameObject.Instantiate(buttonsTMP.gameObject, selectBar, false);
                GameObject potionButton = GameObject.Instantiate(buttonsTMP.gameObject, selectBar, false);
                GameObject specialButton = GameObject.Instantiate(buttonsTMP.gameObject, selectBar, false);

                GameObject.DestroyImmediate(buttonsTMP.gameObject);

                void SetButton(GameObject button, string name, string text)
                {
                    ButtonPF buttonPF = button.AddComponent<ButtonPF>();
                    buttonPF.name = name;
                    buttonPF.transform.SetParent(selectBar, false);
                    RectTransform rect = (RectTransform)buttonPF.transform;
                    rect.anchoredPosition = new Vector2(0f, 0f);
                    rect.anchorMin = new Vector2(0f, 1f);
                    rect.anchorMax = new Vector2(0f, 1f);
                    rect.pivot = new Vector2(.5f, .5f);
                    rect.rotation = Quaternion.identity;
                    rect.sizeDelta = new Vector2(0f, 0f);

                    var orig = GameObject.Instantiate(kmToogle, button.transform, false);
                    DestroyImmediate(orig.GetComponent<LocalizedUIText>());
                    var tmp = orig.GetComponent<TextMeshProUGUI>();
                    var tmphlg = tmp.gameObject.AddComponent<HorizontalLayoutGroup>();
                    tmp.text = text;
                    tmp.alignment = TextAlignmentOptions.Midline;
                    tmp.fontSize = 14f;
                }

                SetButton(spellButton, "spellButton", "Spells");
                SetButton(scrollButton, "scollButton", "Scrolls");
                SetButton(wandButton, "wandButton", "Wands");
                SetButton(potionButton, "potionButton", "Potions");
                SetButton(specialButton, "specialButton", "Special");

                quickWindow.gameObject.SetActive(true);
                quickWindow.localPosition = new Vector3(0f, 0f, 0f);
                window.localScale = new Vector3(_scale, _scale, _scale);
                window.position = Camera.current.ScreenToWorldPoint(new Vector3(Screen.width * .5f, Screen.height * .5f, Camera.current.WorldToScreenPoint(Game.Instance.UI.Common.transform.Find("HUDLayout").position).z));

                Revert = GameObject.Instantiate(window);

                return window.gameObject.AddComponent<UIManager>();
            }
            catch (Exception ex)
            {
                Mod.Error("UI creation failed at: " + ex.Message + ex.StackTrace);
            }
            return new UIManager();
        }

        private void Awake()
        {
            _ownRect = (RectTransform)transform.Find("QuickWindow");
            _ownRect.gameObject.SetActive(true);
            _position = _ownRect.localPosition;
            _buttons = new List<ViewButtonWrapper>();
            _spellViews = new List<RectTransform>();
            CanvasGroup cg;

            var selectBar = (RectTransform)_ownRect.Find("SelectBar");
            foreach (RectTransform t in selectBar)
            {
                _buttons.Add(new ViewButtonWrapper(this, t.gameObject.GetComponent<ButtonPF>(), t.GetSiblingIndex()));
            }
            _buttons[0].IsPressed = true;

            foreach (RectTransform t in _ownRect)
            {
                if (t.name.Contains("SpellView"))
                {
                    cg = t.gameObject.GetComponent<CanvasGroup>();
                    cg.alpha = 0f;
                    _spellViews.Add(t);
                }
            }

            _currentIndex = 0;
            _spellViews[0].SetAsLastSibling();
            cg = _spellViews[0].GetComponent<CanvasGroup>();
            cg.alpha = 1f;

            var moveButton = _ownRect.Find("MoveWindowButton").gameObject;
            moveButton.AddComponent<DraggableWindow>();
            var scaleButton = _ownRect.Find("ScaleWindowButton").gameObject;
            scaleButton.AddComponent<ScalableWindow>();

            new WindowButtonWrapper(_ownRect.Find("MinWindowButton").GetComponent<Button>(), HandleMinimizeOnClick, "Minimize", "Minimizes the window.");
            new WindowButtonWrapper(moveButton.GetComponent<Button>(), HandleMoveDrag, "Move Window", "Click and drag the window to reposition it.");
            new WindowButtonWrapper(scaleButton.GetComponent<Button>(), HandlePriortyOnClick, "Scale Window", "Click and drag to scale the window.");
            new WindowButtonWrapper(_ownRect.Find("SettingsWindowButton").GetComponent<Button>(), HandleSettingsOnClick, "Settings", "Opens the settings window.");

            _minWindow = (RectTransform)transform.Find("Min_Window");
            _minWindow.gameObject.SetActive(true);
            _minWindow.GetComponent<CanvasGroup>().alpha = 1;
            _minWindow.anchoredPosition = new Vector2(-60f, 60f);
            var mindWindowButton = _minWindow.GetComponent<Button>();
            mindWindowButton.onClick = new Button.ButtonClickedEvent();
            mindWindowButton.onClick.AddListener(new UnityAction(HandleMaximizeOnClick));
        }

        private void Update()
        {
        }

        private void HandleMaximizeOnClick()
        {
            StartCoroutine(MaxWindow());
        }

        private IEnumerator MaxWindow()
        {
            var tween = _ownRect.DOScale(1f, .25f).SetUpdate(true);
            _ownRect.DOLocalMove(_position, .25f).SetUpdate(true);
            _ownRect.gameObject.SetActive(true);
            _minWindow.GetComponent<CanvasGroup>().DOFade(0f, .25f);
            yield return tween.WaitForCompletion();
            _minWindow.gameObject.SetActive(false);
        }

        private IEnumerator MinWindow()
        {
            var tween = _ownRect.DOScale(.1f, .25f).SetUpdate(true);
            _ownRect.DOLocalMove(_minWindow.localPosition, .25f).SetUpdate(true);
            _minWindow.gameObject.SetActive(true);
            _minWindow.GetComponent<CanvasGroup>().DOFade(1f, .25f);
            yield return tween.WaitForCompletion();
            _ownRect.gameObject.SetActive(false);
        }

        private void HandleMinimizeOnClick()
        {
            StartCoroutine(MinWindow());
        }

        private void HandleMoveDrag()
        {
        }

        private void HandlePriortyOnClick()
        {
        }

        private void HandleSettingsOnClick()
        {
        }

        private void HandleButtonClick(int index)
        {
            if (index == _currentIndex) return;

            foreach (ViewButtonWrapper b in _buttons) b.IsPressed = false;
            _buttons[index].IsPressed = true;
            _spellViews[index].GetComponent<CanvasGroup>().alpha = 0f;
            _spellViews[index].SetAsLastSibling();
            _spellViews[index].GetComponent<CanvasGroup>().DOFade(1f, .25f).SetUpdate(true);
            _currentIndex = index;
        }

        private class WindowButtonWrapper
        {
            public WindowButtonWrapper(Button button, Action action, string title, string description)
            {
                var _tooltip = button.gameObject.AddComponent<TooltipTrigger>();
                _tooltip.SetNameAndDescription(title, description);
                button.gameObject.AddComponent<OnHover>();
                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(new UnityAction(action));
            }
        }

        public class OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
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

            private readonly ButtonPF _button;
            private readonly Image _image;
            private readonly Sprite _defaultSprite;
            private readonly SpriteState _defaultSpriteState;
            private readonly SpriteState _pressedSpriteState;

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
                            _button.spriteState = _pressedSpriteState;
                            _image.sprite = _pressedSpriteState.pressedSprite;
                        }
                        else
                        {
                            _button.spriteState = _defaultSpriteState;
                            _image.sprite = _defaultSprite;
                        }
                    }
                }
            }

            public ViewButtonWrapper(UIManager ui, ButtonPF button, int index)
            {
                _button = button;
                _button.onClick = new Button.ButtonClickedEvent();
                _button.onClick.AddListener(() => ui.HandleButtonClick(index));
                _image = _button.gameObject.GetComponent<Image>();
                _defaultSprite = BundleManger.LoadedSprites["Log_Toggle_Off"];
                _defaultSpriteState = _button.spriteState;
                _pressedSpriteState = _defaultSpriteState;
                _pressedSpriteState.pressedSprite = BundleManger.LoadedSprites["Log_Toggle_On"];
                _pressedSpriteState.disabledSprite = BundleManger.LoadedSprites["Log_Toggle_On"];
            }
        }
    }
}