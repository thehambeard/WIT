using Kingmaker;
using Kingmaker.GameModes;
using Kingmaker.PubSubSystem;
using Kingmaker.UI;
using Kingmaker.Utility;
using ModMaker;
using QuickCast.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static QuickCast.Utilities.Extensions;

namespace QuickCast.UI.QuickInventory
{
    public class SettingsViewManager : ViewManager, IModEventHandler
    {
        public int Priority => 300;

        private TextMeshProUGUI _textMesh;
        private List<ButtonWrapper> _bindButtons;
        private bool _isBinding = false;
        public static SettingsViewManager CreateObject(MainWindowManager.ViewPortType viewPortType)
        {
            var wrathTMPro = Game.Instance.UI.Canvas.transform?.parent?.Find("LogCanvas/HUDLayout/CombatLog_New/TooglePanel/ToogleAll/ToogleAll/")?.GetComponent<TextMeshProUGUI>() ?? throw new NullReferenceException("wrathTMProMat");

            var settingsView = Game.Instance.UI.Canvas.transform.FirstOrDefault(x => x.name == "Settings");

            var buttonBar = settingsView.Find("Viewport/Content/MinMax/BindButtonBar");
            buttonBar.GetComponentsInChildren<TextMeshProUGUI>().AssignAllFontApperanceProperties(wrathTMPro);

            var bindButtons = buttonBar.GetComponentsInChildren<Button>();

            settingsView.gameObject.SetActive(true);
            var settingsViewMono = settingsView.gameObject.AddComponent<SettingsViewManager>();
            settingsViewMono._viewPortType = viewPortType;
            return settingsViewMono;
        }

        public override void Start()
        {
            base.Start();
            gameObject.SetActive(false);

            if (SetWrap.MinMaxKeyBind == null)
                SetWrap.MinMaxKeyBind = new QCKeyBinding(KeyCode.Z, true, false, false);

            _textMesh = transform.Find("Viewport/Content/MinMax/BindText").GetComponent<TextMeshProUGUI>();
            UpdateMinMaxKeyBindString();

            _bindButtons = new List<ButtonWrapper>();
            _bindButtons.Add(new ButtonWrapper(HandleBindClick, transform.Find("Viewport/Content/MinMax/BindButtonBar/BindButton").GetComponent<Button>()));
            _bindButtons.Add(new ButtonWrapper(HandleAltClick, transform.Find("Viewport/Content/MinMax/BindButtonBar/AltButton").GetComponent<Button>()));
            _bindButtons.Add(new ButtonWrapper(HandleShiftClick, transform.Find("Viewport/Content/MinMax/BindButtonBar/ShiftButton").GetComponent<Button>()));
            _bindButtons.Add(new ButtonWrapper(HandleCtrlClick, transform.Find("Viewport/Content/MinMax/BindButtonBar/CtrlButton").GetComponent<Button>()));

            _bindButtons[1].IsPressed = SetWrap.MinMaxKeyBind.Alt;
            _bindButtons[2].IsPressed = SetWrap.MinMaxKeyBind.Shift;
            _bindButtons[3].IsPressed = SetWrap.MinMaxKeyBind.Ctrl;

            EventBus.Subscribe(this);
        }

        public void Update()
        {
            if (_isBinding && Event.current != null)
            {
                var keyCode = Event.current.keyCode;
                if (keyCode == KeyCode.Escape || keyCode == KeyCode.Backspace)
                {
                    _isBinding = false;
                }
                else if (Event.current.isKey && keyCode != KeyCode.None && !keyCode.IsModifier())
                {
                    SetWrap.MinMaxKeyBind.Key = keyCode;
                    UpdateMinMaxKeyBinding();
                    HandleBindClick();
                }
                UpdateMinMaxKeyBindString();
            }
        }

        private void UpdateMinMaxKeyBinding()
        {
            Game.Instance.Keyboard.UnregisterBinding("MINMAX");
            Game.Instance.Keyboard.RegisterBinding("MINMAX", SetWrap.MinMaxKeyBind.Key, new List<GameModeType>() { GameModeType.Default }, SetWrap.MinMaxKeyBind.Ctrl, SetWrap.MinMaxKeyBind.Alt, SetWrap.MinMaxKeyBind.Shift, KeyboardAccess.TriggerType.KeyDown, KeyboardAccess.ModificationSide.Any);
            UpdateMinMaxKeyBindString();
        }

        private void UpdateMinMaxKeyBindString()
        {
            if (_isBinding)
                _textMesh.text = "Press a key to bind...";
            else
                _textMesh.text = ("Min/Max Hot Key: " + (SetWrap.MinMaxKeyBind.Alt ? "Alt+" : "") + (SetWrap.MinMaxKeyBind.Shift ? "Shift+" : "") + (SetWrap.MinMaxKeyBind.Ctrl ? "Ctrl+" : "") + SetWrap.MinMaxKeyBind.Key.ToString());
        }

        public void HandleBindClick()
        {
            _isBinding = !_isBinding;
            _bindButtons[0].IsPressed = _isBinding;
        }

        public void HandleAltClick()
        {
            SetWrap.MinMaxKeyBind.Alt = !SetWrap.MinMaxKeyBind.Alt;
            _bindButtons[1].IsPressed = SetWrap.MinMaxKeyBind.Alt;
            UpdateMinMaxKeyBinding();
        }

        public void HandleShiftClick()
        {
            SetWrap.MinMaxKeyBind.Shift = !SetWrap.MinMaxKeyBind.Shift;
            _bindButtons[2].IsPressed = SetWrap.MinMaxKeyBind.Shift;
            UpdateMinMaxKeyBinding();
        }

        public void HandleCtrlClick()
        {
            SetWrap.MinMaxKeyBind.Ctrl = !SetWrap.MinMaxKeyBind.Ctrl;
            _bindButtons[3].IsPressed = SetWrap.MinMaxKeyBind.Ctrl;
            UpdateMinMaxKeyBinding();
        }

        public void HandleModEnable()
        {
        }

        public void HandleModDisable() => EventBus.Unsubscribe(this);

        private class ButtonWrapper
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

            public ButtonWrapper(Action action, Button button)
            {
                _button = button;
                _button.onClick = new Button.ButtonClickedEvent();
                _button.onClick.AddListener(new UnityAction(action));
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
