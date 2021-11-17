using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QuickCast.UI.QuickInventory
{
    public class OnHoverCornerButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public bool IsHover { get; private set; }
        public bool IsPressed { get; private set; }

        private CanvasGroup _cg;
        protected virtual void Awake()
        {
            _cg = gameObject.GetComponent<CanvasGroup>();
            _cg.alpha = 0f;
            IsHover = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _cg.DOFade(1f, .25f);
            IsHover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _cg.DOFade(0f, .25f);
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
        //public void LateUpdate()
        //{
        //    if (!IsHover)
        //        gameObject.SetActive(false);
        //}
    }
}
