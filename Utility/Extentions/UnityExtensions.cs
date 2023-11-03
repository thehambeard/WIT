using System.Collections.Generic;
using UnityEngine;

namespace QuickCast.Utility.Extentions
{
    internal static class UnityExtensions
    {
        private static void SafeDestroyInternal(GameObject obj)
        {
            obj.transform.SetParent(null, false);
            obj.SetActive(false);
            Object.Destroy(obj);
        }

        public static void SafeDestroy(this GameObject obj)
        {
            if (obj)
            {
                SafeDestroyInternal(obj);
            }
        }

        public static void SafeDestroy(this Component obj)
        {
            if (obj)
            {
                SafeDestroyInternal(obj.gameObject);
            }
        }

        public static Transform GetParentThenDestory(this Transform obj)
        {
            var parent = obj.parent;
            obj.SafeDestroy();
            return parent;
        }

        public static Transform FindTargetParent(this Transform child, string target)
        {
            Transform result = null;
            Transform parent = child.parent;

            if (child.name == target)
            {
                result = child;
            }
            else if (parent != null)
            {
                result = FindTargetParent(parent, target);
            }
            return result;
        }

        public static List<Transform> FindChildrenRecursive(this Transform root, string name)
        {
            List<Transform> children = new();

            if (null == root)
                return null;

            foreach (Transform child in root)
            {
                if (null == child)
                    continue;
                if (child.name.Equals(name))
                    children.Add(child);
                children.AddRange(FindChildrenRecursive(child, name));
            }
            return children;
        }

        public static void DestroyAllChildren(this Transform go)
        {
            foreach (Transform transform in go)
            {
                transform.gameObject.SafeDestroy();
            }
        }

        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            Vector3 deltaPosition = rectTransform.pivot - pivot;    // get change in pivot
            deltaPosition.Scale(rectTransform.rect.size);           // apply sizing
            deltaPosition.Scale(rectTransform.localScale);          // apply scaling
            deltaPosition = rectTransform.rotation * deltaPosition; // apply rotation

            rectTransform.pivot = pivot;                            // change the pivot
            rectTransform.localPosition -= deltaPosition;           // reverse the position change
        }
    }
}
