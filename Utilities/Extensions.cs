using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WIT.Utilities
{
    public static class Extensions
    {
        public static RectTransform FindTargetParent(this Transform child, string target)
        {
            Transform result = null;
            Transform parent = child.parent;

            Main.Mod.Debug($"Child: {child} Parent: {parent} Target{target}");
            if (child.name == target)
            {
                result = child;
            }
            else if (parent != null)
            {
                result = FindTargetParent(parent, target);
            }
            return (RectTransform) result;
        }
        public static RectTransform FindChildStartsWith(this Transform transform, string path, string search)
        {
            foreach(RectTransform t in transform.Find(path))
            {
                if (t.name.StartsWith(search))
                    return t;
            }
            return null;
        }
        
        public static Transform FirstOrDefault(this Transform transform, Func<Transform, bool> query)
        {
            if (query(transform))
            {
                return transform;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                var result = FirstOrDefault(transform.GetChild(i), query);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static RectTransform FindChildContains(this Transform transform, string path, string search)
        {
            foreach (RectTransform t in transform.Find(path))
            {
                if (t.name.Contains(search))
                    return t;
            }
            return null;
        }

        //not recursive
        public static List<RectTransform> FindChildrenStartsWith(this Transform transform, string path, string search)
        {
            List<RectTransform> result = new List<RectTransform>();

            foreach (RectTransform t in transform.Find(path))
            {
                if (t.name.StartsWith(search))
                    result.Add(t);
            }
            return result;
        }

        //not recursive
        public static List<RectTransform> FindChildrenContainsWith(this Transform transform, string path, string search)
        {
            List<RectTransform> result = new List<RectTransform>();

            foreach (RectTransform t in transform.Find(path))
            {
                if (t.name.Contains(search))
                    result.Add(t);
            }
            return result;
        }

        public static TextMeshProUGUI AssignFontApperanceProperties(this TextMeshProUGUI tmp, TextMeshProUGUI source)
        {
            if (tmp == null) return null;

            tmp.color = source.color;
            tmp.colorGradient = source.colorGradient;
            tmp.colorGradientPreset = source.colorGradientPreset;
            tmp.font = source.font;
            tmp.fontMaterial = source.fontMaterial;
            tmp.fontStyle = source.fontStyle;
            tmp.fontWeight = source.fontWeight;
            tmp.outlineColor = source.outlineColor;
            tmp.outlineWidth = source.outlineWidth;
            tmp.faceColor = source.faceColor;

            return tmp;
        }

        public static TextMeshProUGUI[] AssignAllFontApperanceProperties(this TextMeshProUGUI[] tmp, TextMeshProUGUI source)
        {
            if (tmp == null) return null;

            for(int i = 0; i < tmp.Length; i++)
            {
                tmp[i].AssignFontApperanceProperties(source);
            }
            return tmp;
        }
    }
}