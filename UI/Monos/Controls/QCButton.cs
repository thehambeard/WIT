using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QuickCast.UI.Utility
{
    public class QCButton : Button
    {
        public bool IsToggable;
        public string DefaultText;
        public string ToggleText;
        public Sprite DefaultSprite;

        private bool _isToggled;
        protected Image _image;
        private TextMeshProUGUI _textMesh;

        public UnityEvent OnRightClickEvent = new();
        public UnityEvent OnPointerEnterEvent = new();
        public UnityEvent OnPointerExitEvent = new();

        public bool IsToggled
        {
            get
            {
                return _isToggled;
            }

            set
            {
                if (_isToggled != value)
                {
                    _isToggled = value;

                    if (value)
                    {
                        switch (transition)
                        {
                            case Transition.None:
                                break;
                            case Transition.ColorTint:
                                _image.color = colors.pressedColor;
                                break;
                            case Transition.SpriteSwap:
                                _image.sprite = spriteState.pressedSprite;
                                break;
                        }

                        if (_textMesh != null)
                            _textMesh.text = ToggleText;
                    }
                    else
                    {
                        switch (transition)
                        {
                            case Transition.None:
                                break;
                            case Transition.ColorTint:
                                _image.color = colors.normalColor;
                                break;
                            case Transition.SpriteSwap:
                                _image.sprite = DefaultSprite;
                                break;
                        }

                        if (_textMesh != null)
                            _textMesh.text = DefaultText;
                    }
                }
            }
        }

        public virtual void Initialize()
        {
            _image = GetComponent<Image>();
            _textMesh = gameObject.GetComponentInChildren<TextMeshProUGUI>();

            if (_image != null)
                _image.sprite = DefaultSprite;

            if (_textMesh != null)
                _textMesh.text = DefaultText;

            IsToggled = false;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    if (IsToggable)
                        IsToggled = !IsToggled;
                    base.OnPointerClick(eventData);
                    break;
                case PointerEventData.InputButton.Right:
                    if (OnRightClickEvent != null)
                        OnRightClickEvent.Invoke();
                    break;
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (OnPointerEnterEvent != null)
                OnPointerEnterEvent.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (OnPointerExitEvent != null)
                OnPointerExitEvent.Invoke();
        }
    }
}
