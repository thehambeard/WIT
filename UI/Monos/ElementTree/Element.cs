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
        protected readonly SortedDictionary<string, Element> _children = new();
        protected int _subLevel = 0;
        protected bool _claiming = false;

        public Element Parent;
        public bool ShowIfChildless = true;
        public bool ShowIfUnclaim = true;
        public bool AllowUnclaim = true;
        public bool IsExpanded { get; protected set; } = true;
        public bool HasElements => _children.Count > 0;
        public bool Claiming => _claiming;

        public abstract void Initialize();

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
                element._subLevel++;
                _children.Add(key, element);
            }
        }

        public virtual void Remove(string key)
        {
            if (_children.ContainsKey(key))
            {
                _children[key].Parent = null;
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

            if (!HasElements && !ShowIfChildless)
            {
                if (gameObject.activeSelf)
                    gameObject.SetActive(false);

                return;
            }

            transform.SetAsLastSibling();

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            _claiming = true;
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
    }
}
