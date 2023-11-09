using QuickCast.UI.Monos.ElementTree;
using QuickCast.UI.Monos.ViewControlGroup.SpellSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            foreach(var element in elements) 
            {
                element.Claim();
            }
        }
    }
}
