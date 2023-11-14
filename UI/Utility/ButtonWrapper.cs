using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;

namespace QuickCast.UI.Utility
{
    public class ButtonWrapper : MonoBehaviour,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerUpHandler,
        IPointerDownHandler
    {
        public string DefaultText;
        public string PressedText;
        public bool IsToggle;

        public UnityEvent OnLeftClickEvent = new();
        public UnityEvent OnRightClickEvent = new();
        public UnityEvent OnPointerEnterEvent = new();
        public UnityEvent OnPointerExitEvent = new();

        private bool _isPressed;
        private Button _button;
        protected Image _image;
        private TextMeshProUGUI _textMesh;
        
        private SpriteState _pressedState;
        private SpriteState _defaultState;
        protected Sprite _defaultSprite;
        protected Sprite _pressedSprite;

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
                        _button.spriteState = _pressedState;
                        //_image.sprite = _pressed.pressedSprite;

                        if(_textMesh != null && PressedText != null) 
                            _textMesh.text = PressedText;
                    }
                    else
                    {
                        _button.spriteState = _defaultState;
                        //_image.sprite = _defaultSprite;

                        if (_textMesh != null && PressedText != null)
                            _textMesh.text = DefaultText;
                    }
                }
            }
        }

        public virtual void Initialize()
        {
            _button = gameObject.GetComponent<Button>();
            _textMesh = gameObject.GetComponentInChildren<TextMeshProUGUI>();

            if (_textMesh != null)
                _textMesh.text = DefaultText;

            _image = _button.gameObject.GetComponent<Image>();
            _defaultSprite = _image?.sprite;
            //_pressedSprite = _button.spriteState.pressedSprite;
            _defaultState = _button.spriteState;
            _pressedState = _defaultState;
            //_pressed.disabledSprite = _pressed.pressedSprite;
            //_pressed.highlightedSprite = _pressed.pressedSprite;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (OnLeftClickEvent != null && eventData.button == PointerEventData.InputButton.Left)
            {
                OnLeftClickEvent.Invoke();

                if (IsToggle) IsPressed = !IsPressed;
            }
            else if (OnRightClickEvent != null && eventData.button != PointerEventData.InputButton.Right) 
            {
                OnRightClickEvent.Invoke();
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (OnPointerEnterEvent != null)
                OnPointerEnterEvent.Invoke();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (OnPointerExitEvent != null)
                OnPointerExitEvent.Invoke();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if(!IsToggle) IsPressed = !IsPressed;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!IsToggle) IsPressed = !IsPressed;
        }
    }
}
