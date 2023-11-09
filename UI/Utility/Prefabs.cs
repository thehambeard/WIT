using Owlcat.Runtime.Core.Utils;
using QuickCast.UI.Monos.Controls;
using QuickCast.UI.Monos;
using QuickCast.Utility;
using QuickCast.Utility.Extentions;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Collections.Generic;
using QuickCast.UI.Monos.ViewControlGroup.SpellSV;
using QuickCast.UI.Monos.ViewControlGroup;

namespace QuickCast.UI.Utility
{
    internal static class Prefabs
    {
        public static GameObject Main => Get("QuickCast");
        public static GameObject WindowControlGroup => Get("WindowControlGroup");
        public static GameObject ScrollView => Get("ScrollView");
        public static GameObject ViewControlGroup => Get("ViewControlGroup");
        public static GameObject SpellElement => Get("SpellElement");
        public static GameObject LevelHeaderElement => Get("LevelHeaderElement");
        public static GameObject BookHeaderElement => Get("BookHeaderElement");
        public static GameObject ScrollViewMode => Get("ScrollViewMode");
        public static void Initialize()
        {
            RemoveReferences();
        }

        public static GameObject Get(string name)
        {
            return AssetBundleManager.Instance.Prefabs[name];
        }

        private static void RemoveReferences()
        {
            var fabs = AssetBundleManager.Instance.Prefabs;
            var fabNames = fabs.Keys.ToList();

            foreach (var fab in fabs.Values)
            {
                foreach (var fabName in fabNames)
                {
                    if (fab.name == fabName)
                    {
                        fab.transform.SetParent(null);
                        continue;
                    }

                    Transform result;
                    while ((result = fab.transform.FindChildRecursive(fabName)) != null)
                        result.SafeDestroy();
                }
            }
        }
    }
}