using Kingmaker;
using Kingmaker.Blueprints.Root;
using Kingmaker.UI;
using Kingmaker.UI.Log;
using QuickCast.Utility.Extentions;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QuickCast.UI.Monos.Controls
{
    internal class QCResizePanel : MonoBehaviour,
    IEventSystemHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IDragHandler
    {
        public Vector2 MaxSize = new Vector2(400f, 400f);
        public Vector2 MinSize = new Vector2(100f, 100f);
        private Vector2 _originalLocalPointerPosition;
        private Vector2 _originalSizeDelta;
        public ResizePivot Pivot;
        public GameObject TargetGameObject;
        private IResizeElement _target;
        private bool _isInteractable = true;
        private bool _isDrag;

        private IResizeElement Target => _target = _target ?? TargetGameObject.GetComponent<IResizeElement>();

        public void OnDrag(PointerEventData data)
        {
            if (!_isInteractable || Target == null)
                return;
            _isDrag = true;
            ShowVerticalCursor();
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Target.GetTransform(), data.position, data.pressEventCamera, out localPoint);
            Vector2 size = _originalSizeDelta + GetOffset(localPoint);
            size = new Vector2((float)(int)Mathf.Clamp(size.x, MinSize.x, MaxSize.x), (float)(int)Mathf.Clamp(size.y, MinSize.y, MaxSize.y));
            Target.SetSizeDelta(size);
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (!_isDrag)
                return;
            _isDrag = false;
            HideVerticalCursor();

            Target.GetTransform().SetPivot(new Vector2(.5f, .5f));
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (!_isInteractable)
                return;

            switch (Pivot)
            {
                case ResizePivot.Bottom:
                    Target.GetTransform().SetPivot(new Vector2(.5f, 1f));
                    break;
                case ResizePivot.Top:
                    Target.GetTransform().SetPivot(new Vector2(.5f, 0f));
                    break;
                case ResizePivot.Right:
                    Target.GetTransform().SetPivot(new Vector2(0f, .5f));
                    break;
                case ResizePivot.Left:
                    Target.GetTransform().SetPivot(new Vector2(1f, .5f));
                    break;
            }

            _originalSizeDelta = Target.GetSize();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Target.GetTransform(), data.position, data.pressEventCamera, out _originalLocalPointerPosition);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isInteractable)
                return;
            ShowVerticalCursor();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isInteractable || _isDrag)
                return;
            HideVerticalCursor();
        }

        private void ShowVerticalCursor()
        {
            if (CursorController.IsResizeCursor)
                return;
            Game.Instance.CursorController.SetCustomCursor(GetCursor(), new Vector2(32f, 32f));
            CursorController.IsResizeCursor = true;
        }

        private void HideVerticalCursor()
        {
            if (!CursorController.IsResizeCursor)
                return;
            CursorController.IsResizeCursor = false;
            Game.Instance.CursorController.ClearCursor();
            Game.Instance.CursorController.SetCustomCursor(CursorRoot.CursorType.None, Vector2.zero);
        }

        private CursorRoot.CursorType GetCursor()
        {
            switch (Pivot)
            {
                case ResizePivot.TopLeft:
                case ResizePivot.BottomRight:
                    return CursorRoot.CursorType.ArrowDiagonally02Cursor;
                case ResizePivot.Top:
                case ResizePivot.Bottom:
                    return CursorRoot.CursorType.ArrowVerticalCursor;
                case ResizePivot.TopRight:
                case ResizePivot.BottomLeft:
                    return CursorRoot.CursorType.ArrowDiagonally01Cursor;
                case ResizePivot.Left:
                case ResizePivot.Right:
                    return CursorRoot.CursorType.ArrowHorizontalCursor;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Vector2 GetOffset(Vector2 localPointerPosition)
        {
            Vector3 vector3 = (Vector3)(localPointerPosition - _originalLocalPointerPosition);
            float x = 0.0f;
            float y = 0.0f;
            switch (Pivot)
            {
                case ResizePivot.TopLeft:
                    x = -vector3.x;
                    y = vector3.y;
                    break;
                case ResizePivot.Top:
                    y = vector3.y;
                    break;
                case ResizePivot.TopRight:
                    x = vector3.x;
                    y = vector3.y;
                    break;
                case ResizePivot.Left:
                    x = -vector3.x;
                    break;
                case ResizePivot.Right:
                    x = vector3.x;
                    break;
                case ResizePivot.BottomLeft:
                    x = -vector3.x;
                    y = -vector3.y;
                    break;
                case ResizePivot.Bottom:
                    y = -vector3.y;
                    break;
                case ResizePivot.BottomRight:
                    x = vector3.x;
                    y = -vector3.y;
                    break;
            }
            return new Vector2(x, y);
        }

        public enum ResizePivot
        {
            TopLeft,
            Top,
            TopRight,
            Left,
            Right,
            BottomLeft,
            Bottom,
            BottomRight,
        }

        public void Start()
        {
            TargetGameObject = transform.FindTargetParent("QuickCast").gameObject;
            MaxSize = new Vector2(1500, 4000);
            MinSize = new Vector2(600, 600);
        }
    }
}
