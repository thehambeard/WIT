using DG.Tweening;
using Kingmaker.UI.Tooltip;
using QuickCast.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QuickCast.UI.QuickInventory
{
    public class ScalableWindow : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
    {
        private bool _moveMode;
        private Vector2 _mouseStartPos;
        private Vector2 _lastMausePos;
        private TooltipTrigger _tooltip;
        private RectTransform _ownRectTransform;
        private Vector2 _currentScale;

        private void Start()
        {
            _ownRectTransform = transform.FindTargetParent("QuickInventory");
            _tooltip = gameObject.GetComponent<TooltipTrigger>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            _moveMode = true;
            _mouseStartPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            _ownRectTransform.DOScale(_ownRectTransform.localScale, 0.1f).SetUpdate(true);
            _currentScale = _ownRectTransform.localScale;
            _tooltip.enabled = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            _ownRectTransform.DOScale(_ownRectTransform.localScale, 0.1f).SetUpdate(true);
            _moveMode = false;
            _mouseStartPos = default(Vector2);
            _tooltip.enabled = true;
            SetWrap.Window_Pos = _ownRectTransform.localPosition;
            SetWrap.Window_Scale = _ownRectTransform.localScale;
        }

        public void LateUpdate()
        {
            if (!_moveMode)
            {
                return;
            }
            Vector2 vector = new Vector2(Input.mousePosition.x - _mouseStartPos.x, Input.mousePosition.y - _mouseStartPos.y);
            if (_lastMausePos == vector)
            {
                return;
            }
            var x = vector.x;

            if (x < 100f && x > 0f)
            {
                _ownRectTransform.localScale = HamHelpers.MapValueVector(0f, 100f, _currentScale.x, 2f, x);
            }
            if (x > -100f && x < 0f)
            {
                _ownRectTransform.localScale = HamHelpers.MapValueVector(0f, -100f, _currentScale.x, .4f, x);
            }
            _lastMausePos = vector;
        }
    }
}