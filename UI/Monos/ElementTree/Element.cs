using Kingmaker.EntitySystem.Entities;
using QuickCast.Utility.Extentions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QuickCast.UI.Monos.ElementTree
{
    internal abstract class Element : MonoBehaviour
    {
        protected readonly SortedList<string, Element> _children = new();
        
        protected bool _claiming = false;
        protected float _indentOffset = 12f;

        public Element Parent;
        public int SubLevel = 0;
        public bool ShowIfChildless = true;
        public bool ShowIfUnclaim = true;
        public bool AllowUnclaim = true;
        public UnitEntityData Unit;
        public bool IsExpanded { get; protected set; } = true;
        public bool HasElements => _children.Count > 0;
        public bool IsClaiming => _claiming;

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

        public virtual void Show(bool active)
        {
            TraverseChildren((x) => x.ShowAction(active));
        }

        private void ShowAction(bool active)
        {
            if (HasElements || ShowIfChildless)
                gameObject.SetActive(active && Parent.IsExpanded);
            else if (!active)
                gameObject.SetActive(active && Parent.IsExpanded);
        }

        public virtual void OnExpandToggle()
        {
            IsExpanded = !IsExpanded;
            Show(IsExpanded);
        }

        private void ClaimAction()
        {
            if (_claiming && AllowUnclaim) { return; }

            _claiming = true;
            transform.SetAsLastSibling();
            SetElementLayout();

            foreach (var child in _children)
            {
                child.Value.Parent = this;
                child.Value.SubLevel = SubLevel + 1;
            }

            if ((Parent != null && !Parent.IsExpanded) || (!HasElements && !ShowIfChildless))
            {
                if (gameObject.activeSelf)
                    gameObject.SetActive(false);

                return;
            }

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }

        public virtual void UnclaimAction()
        {
            if (!_claiming || !AllowUnclaim) { return; }

            if (!ShowIfUnclaim && gameObject.activeSelf)
                gameObject.SetActive(false);

            _claiming = false;
        }

        public virtual void Unclaim()
        {
            Traverse((x) => x.UnclaimAction());
        }

        public virtual void Claim()
        {
            Traverse((x) => x.ClaimAction());
        }

        public virtual void Traverse(Action<Element> action)
        {
            action(this);
            TraverseChildren(action);
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
