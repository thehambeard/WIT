using QuickCast.UI.Monos.ElementTree;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuickCast.UI.Monos.ViewControlGroup
{
    internal abstract class SVManager : MonoBehaviour
    {
        protected ScrollRect _scrollRect;

        public virtual void Initialize()
        {
            _scrollRect = GetComponent<ScrollRect>();
        }

        public void SortBy<T>(List<T> elements) where T : Element
        {
            foreach (var element in elements)
            {
                element.Claim();
            }
        }
    }
}
