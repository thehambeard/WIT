using DG.Tweening;
using Kingmaker;
using Kingmaker.UI.Tooltip;
using QuickCast.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QuickCast.UI.QuickInventory
{
    public class DraggableWindow : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
    {
        private bool _MoveMode;
        private Vector2 _MouseStartPos;
        private Vector2 _ContainerStartPos;
        private Vector2 _LastMausePos;
        private Vector2 _TakeDrag;
        private RectTransform _OwnRectTransform;
        private RectTransform _ParentRectTransform;
        private TooltipTrigger _Tooltip;
        private float _scaleFactor;
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            _MoveMode = true;
            _MouseStartPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            _OwnRectTransform.anchoredPosition = _OwnRectTransform.anchoredPosition + _TakeDrag;
            _OwnRectTransform.DOAnchorPos(_OwnRectTransform.anchoredPosition + _TakeDrag, 0.1f, false).SetUpdate(true);
            _ContainerStartPos = _OwnRectTransform.anchoredPosition;
            if (_Tooltip != null)
                _Tooltip.enabled = false;
            _scaleFactor = Game.Instance.UI.Canvas.GetComponent<Canvas>().scaleFactor;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _OwnRectTransform.DOAnchorPos(_OwnRectTransform.anchoredPosition - _TakeDrag, 0.1f, false).SetUpdate(true);
            _MoveMode = false;
            _MouseStartPos = default(Vector2);
            if (_Tooltip != null)
                _Tooltip.enabled = true;
            SetWrap.Window_Pos = _OwnRectTransform.localPosition;
            SetWrap.Window_Scale = _OwnRectTransform.localScale;
        }

        public void LateUpdate()
        {
            if (!_MoveMode)
            {
                return;
            }
            Vector2 vector2 = new Vector2((Input.mousePosition.x - _MouseStartPos.x) / _scaleFactor, (Input.mousePosition.y - _MouseStartPos.y) / _scaleFactor);
            if (_LastMausePos == vector2)
            {
                return;
            }
            Vector2 vector3 = _ContainerStartPos + vector2 - _TakeDrag;
            _OwnRectTransform.anchoredPosition = (vector3 + _TakeDrag);
            _LastMausePos = vector2;
        }

        private void Start()
        {
            _TakeDrag = new Vector2(0f, 0f);
            _OwnRectTransform = transform.FindTargetParent("QuickInventory");
            _ParentRectTransform = (RectTransform)_OwnRectTransform.parent;
            _Tooltip = gameObject.GetComponent<TooltipTrigger>();
        }
    }
}