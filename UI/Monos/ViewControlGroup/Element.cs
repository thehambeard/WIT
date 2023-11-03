using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QuickCast.UI.Monos.ViewControlGroup
{
    internal class Element : MonoBehaviour
    {
        protected SortedSet<Element> SubElements = new(new ElementCompare());
        protected int SubLevel = 0;

        private bool _claiming = false;

        public void Add(Element element)
        {
            if (!SubElements.Contains(element))
            {
                SubElements.Add(element);

                if (_claiming)
                {
                    Insert(element);

                    if (!gameObject.activeSelf)
                        gameObject.SetActive(true);
                }
            }
        }

        public void Remove(Element element)
        {
            if (SubElements.Contains(element))
            {
                SubElements.Remove(element);

                if (SubElements.Count == 0)
                    gameObject.SetActive(false);
            }
        }

        public void Claim()
        {
            _claiming = true;

            if (SubElements.Count == 0) return;

            gameObject.SetActive(true);

            Sort();
        }

        private void Insert(Element element)
        {
            int index = transform.GetSiblingIndex() + 1;
            var comp = new ElementCompare();

            foreach (var e in SubElements)
            {
                if (comp.Compare(element, e) >= 0)
                    break;
                index++;
            }

            element.transform.SetSiblingIndex(index);
        }

        public void Sort()
        {
            int index = transform.GetSiblingIndex() + 1;

            foreach (var element in SubElements)
                element.transform.SetSiblingIndex(index++);
        }

        public void Unclaim()
        {
            _claiming = false;
            gameObject?.SetActive(false);
        }
    }

    internal class ElementCompare : IComparer<Element>
    {
        public int Compare(Element x, Element y)
        {
            return x.name.CompareTo(y.name);
        }
    }
}
