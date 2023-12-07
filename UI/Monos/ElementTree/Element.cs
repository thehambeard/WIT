using Kingmaker.EntitySystem.Entities;
using QuickCast.Utility.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuickCast.UI.Monos.ElementTree
{
    internal abstract class Element : MonoBehaviour
    {
        protected readonly SortedList<string, Element> _children = new();
        protected float _indentOffset = 12f;

        public Element Parent;
        public int SubLevel = 0;
        public bool ShowIfChildless = true;
        public bool ShowIfUnclaimed = true;
        public bool AllowUnclaim = true;
        public UnitEntityData Unit;

        public bool IsExpanded { get; protected set; } = true;
        public bool IsClaiming { get; protected set; } = false;
        public bool IsHidden { get; protected set; } = false;
        public bool HasElements => _children.Count > 0;
        public bool HasActiveElements
        {
            get
            {
                bool active = false;
                foreach (var element in _children.Values)
                {
                    if (element.gameObject.activeSelf || element.HasActiveElements)
                    {
                        active = true;
                        break;
                    }
                }

                return active;
            }
        }


        public abstract void Initialize();
        public abstract void SetElementLayout();

        public Element this[string key]
        {
            get
            {
                return _children[key];
            }
        }

        public virtual void Add(string key, Element element)
        {
            if (!_children.ContainsKey(key))
            {
                element.Parent = this;
                element.SubLevel = SubLevel + 1;
                _children.Add(key, element);

                var index = _children.IndexOfKey(key);
                _children[key].transform.SetSiblingIndex(index);
            }
        }

        public virtual void Remove(string key)
        {
            if (_children.ContainsKey(key))
            {
                _children[key].Parent = null;
                _children[key].SafeDestroy();
                _children.Remove(key);
            }
        }

        public virtual void SetHidden(bool hidden)
        {
            Traverse((x) =>
            {
                x.IsHidden = hidden;
            });

            Draw();
        }

        public virtual bool UpdateActive()
        {
            bool show = !IsHidden && (Parent == null ? true : Parent.IsExpanded) && (HasElements || ShowIfChildless) && (IsClaiming || ShowIfUnclaimed);

            if (gameObject.activeSelf != show)
                gameObject.SetActive(show);

            return show;
        }

        public virtual void Draw()
        {
            ReverseTraverse((x) =>
            {
                x.UpdateActive();
            });

        }

        public virtual void OnExpandToggle()
        {
            IsExpanded = !IsExpanded;

            Draw();
        }

        private void ClaimAction()
        {
            if (IsClaiming && AllowUnclaim) { return; }

            IsClaiming = true;
            transform.SetAsLastSibling();
            SetElementLayout();

            foreach (var child in _children)
            {
                child.Value.Parent = this;
                child.Value.SubLevel = SubLevel + 1;
            }
        }

        public virtual void UnclaimAction()
        {
            if (!IsClaiming || !AllowUnclaim) { return; }
            IsClaiming = false;
        }

        public virtual void Unclaim()
        {
            Traverse((x) => x.UnclaimAction());
            Draw();
        }

        public virtual void Claim()
        {
            Traverse((x) => x.ClaimAction());
            Draw();
        }

        public virtual void Traverse(Action<Element> action)
        {
            action(this);
            TraverseChildren(action);
        }

        public virtual void ReverseTraverse(Action<Element> action)
        {
            ReverseTraverseChildren(action);
            action(this);
        }

        public virtual void ReverseTraverseChildren(Action<Element> action)
        {
            foreach (var child in _children.Reverse())
                child.Value.Traverse(action);
        }

        public virtual void TraverseChildren(Action<Element> action)
        {
            foreach (var child in _children)
                child.Value.Traverse(action);
        }

        protected void SetMin(RectTransform rect, float min)
        {
            rect.offsetMin = new(min + (_indentOffset * SubLevel), rect.offsetMin.y);
        }

        protected void SetMax(RectTransform rect, Vector2 sizeDelta)
        {
            rect.offsetMax = new(sizeDelta.x + (_indentOffset * SubLevel), rect.offsetMax.y);
        }
    }
}
