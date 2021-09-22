using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WIT.Utilities
{
    public static class Extensions
    {
        public static Transform FindChildStartsWith(this Transform transform, string search)
        {
            foreach(Transform t in transform)
            {
                if (t.name.StartsWith(search))
                    return t;
            }
            return null;
        }

        public static Transform FindChildContains(this Transform transform, string search)
        {
            foreach (Transform t in transform)
            {
                if (t.name.Contains(search))
                    return t;
            }
            return null;
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