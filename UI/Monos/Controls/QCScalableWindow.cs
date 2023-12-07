using DG.Tweening;
using Kingmaker;
using Kingmaker.Blueprints.Root;
using Kingmaker.UI;
using QuickCast.Utility;
using QuickCast.Utility.Extentions;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QuickCast.UI.Monos.Controls
{
    internal class QCScalableWindow : MonoBehaviour,
        IPointerDownHandler,
        IEventSystemHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public enum WindowCorner
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public WindowCorner ScaleCorner;

        private bool _moveMode;
        private Vector2 _mouseStartPos;
        private Vector2 _lastMausePos;
        private RectTransform _ownRectTransform;
        private Vector2 _currentScale;
        private float _minScale = .3f;
        private float _maxScale = 2f;

        private void Start()
        {
            _ownRectTransform = (RectTransform)transform.FindTargetParent("QuickCast");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            _moveMode = true;
            _mouseStartPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            _ownRectTransform.DOScale(_ownRectTransform.localScale, 0.075f).SetUpdate(true);
            _currentScale = _ownRectTransform.localScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            _ownRectTransform.DOScale(_ownRectTransform.localScale, 0.075f).SetUpdate(true);
            _moveMode = false;
            _mouseStartPos = default(Vector2);
            HideCursor();
        }

        public void LateUpdate()
        {
            if (!_moveMode)
            {
                return;
            }
            ShowCursor();
            Vector2 vector = new Vector2(Input.mousePosition.x - _mouseStartPos.x, Input.mousePosition.y - _mouseStartPos.y);
            if (_lastMausePos == vector)
            {
                return;
            }
            var x = vector.x;

            if (x < 100f && x > 0f)
            {
                _ownRectTransform.localScale = Helpers.MapValueVector(0f, 100f, _currentScale.x, _maxScale, x);
            }
            if (x > -100f && x < 0f)
            {
                _ownRectTransform.localScale = Helpers.MapValueVector(0f, -100f, _currentScale.x, _minScale, x);
            }
            _lastMausePos = vector;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowCursor();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_moveMode)
                return;
            HideCursor();
        }

        private void ShowCursor()
        {
            if (CursorController.IsResizeCursor)
                return;
            Game.Instance.CursorController.SetCustomCursor(GetCursor(), new Vector2(32f, 32f));
            CursorController.IsResizeCursor = true;
        }

        private void HideCursor()
        {
            if (!CursorController.IsResizeCursor)
                return;
            CursorController.IsResizeCursor = false;
            Game.Instance.CursorController.ClearCursor();
            Game.Instance.CursorController.SetCustomCursor(CursorRoot.CursorType.None, Vector2.zero);
        }

        private CursorRoot.CursorType GetCursor()
        {
            switch (ScaleCorner)
            {
                case WindowCorner.TopLeft:
                case WindowCorner.BottomRight:
                    return CursorRoot.CursorType.ArrowDiagonally02Cursor;
                case WindowCorner.TopRight:
                case WindowCorner.BottomLeft:
                    return CursorRoot.CursorType.ArrowDiagonally01Cursor;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
