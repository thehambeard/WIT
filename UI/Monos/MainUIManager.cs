using Kingmaker.UI.Log;
using QuickCast.UI.Monos.ViewControlGroup;
using UnityEngine;

namespace QuickCast.UI.Monos
{
    internal class MainUIManager : MonoBehaviour, IResizeElement
    {
        public States.WindowState WindowState { get; private set; }
        public States.SelectState SelectState { get; private set; }
        public static MainUIManager Instance { get; private set; }

        public void Awake()
        {
            Instance = this;

            var rect = (RectTransform)transform;
            rect.sizeDelta = new Vector2(750f, 1200);
            rect.pivot = new Vector2(0.5f, 0.5f);
            transform.localScale = new Vector3(.5f, .5f, .5f);
            transform.localPosition = new Vector3(0, 0);

            WindowState = States.WindowState.Maximized;
            SelectState = States.SelectState.Spells;
        }

        public Vector2 GetSize() => ((RectTransform)transform).sizeDelta;

        public RectTransform GetTransform() => (RectTransform)transform;

        public void SetSizeDelta(Vector2 size)
        {
            ((RectTransform)transform).sizeDelta = size;
        }
    }
}
