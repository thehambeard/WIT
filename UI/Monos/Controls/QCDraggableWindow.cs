using DG.Tweening;
using Kingmaker;
using QuickCast.Utility.Extentions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QuickCast.UI.Monos.Controls
{
    internal class QCDraggableWindow : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
    {
        private bool _moveMode;
        private Vector2 _mouseStartPos;
        private Vector2 _containerStartPos;
        private Vector2 _lastMausePos;
        private Vector2 _takeDrag;
        private RectTransform _ownRectTransform;
        private float _scaleFactor;
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            _moveMode = true;
            _mouseStartPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            _ownRectTransform.anchoredPosition = _ownRectTransform.anchoredPosition + _takeDrag;
            _ownRectTransform.DOAnchorPos(_ownRectTransform.anchoredPosition + _takeDrag, 0.1f, false).SetUpdate(true);
            _containerStartPos = _ownRectTransform.anchoredPosition;
            _scaleFactor = Game.Instance.UI.Canvas.GetComponent<Canvas>().scaleFactor;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _ownRectTransform.DOAnchorPos(_ownRectTransform.anchoredPosition - _takeDrag, 0.1f, false).SetUpdate(true);
            _moveMode = false;
            _mouseStartPos = default(Vector2);
            //SetWrap.Window_Pos = _OwnRectTransform.localPosition;
            //SetWrap.Window_Scale = _OwnRectTransform.localScale;
        }

        public void LateUpdate()
        {
            if (!_moveMode)
            {
                return;
            }
            Vector2 vector2 = new Vector2((Input.mousePosition.x - _mouseStartPos.x) / _scaleFactor, (Input.mousePosition.y - _mouseStartPos.y) / _scaleFactor);
            if (_lastMausePos == vector2)
            {
                return;
            }
            Vector2 vector3 = _containerStartPos + vector2 - _takeDrag;
            _ownRectTransform.anchoredPosition = (vector3 + _takeDrag);
            _lastMausePos = vector2;
        }

        public void Start()
        {
            _takeDrag = new Vector2(0f, 0f);
            _ownRectTransform = (RectTransform)transform.FindTargetParent("QuickCast");
        }
    }
}
