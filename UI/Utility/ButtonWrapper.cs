using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace QuickCast.UI.Utility
{
    public class ButtonWrapper
    {
        private bool _isPressed;

        private readonly Button _button;
        private readonly Image _image;
        private readonly TextMeshProUGUI _textMesh;
        private readonly string _defaultText;
        private readonly string _pressedText;

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
                        //_image.sprite = _pressed.pressedSprite;

                        if(_textMesh != null && _pressedText != null) 
                            _textMesh.text = _pressedText;
                    }
                    else
                    {
                        _button.spriteState = _default;
                        //_image.sprite = _defaultSprite;

                        if (_textMesh != null && _pressedText != null)
                            _textMesh.text = _defaultText;
                    }
                }
            }
        }

        public ButtonWrapper(Transform button, Action action, string pressedText = null)
        {
            _button = button.gameObject.GetComponentInChildren<Button>();
            _textMesh = button.gameObject.GetComponentInChildren<TextMeshProUGUI>();

            if (_textMesh != null)
            {
                _defaultText = _textMesh.text;
                _pressedText = pressedText;
            }

            _button.onClick = new Button.ButtonClickedEvent();
            _button.onClick.AddListener(new UnityAction(action));
            _image = _button.gameObject.GetComponent<Image>();
            _defaultSprite = _image.sprite;
            //_pressedSprite = _button.spriteState.pressedSprite;
            _default = _button.spriteState;
            _pressed = _default;
            //_pressed.disabledSprite = _pressed.pressedSprite;
            //_pressed.highlightedSprite = _pressed.pressedSprite;
        }
    }
}
