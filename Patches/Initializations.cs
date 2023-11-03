using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using QuickCast.UI.Utility;
using QuickCast.Utility;
using System.IO;

namespace QuickCast.Patches
{
    internal class Initializations
    {
        [HarmonyPatch(typeof(BlueprintsCache))]
        static class BlueprintsCache_Patch
        {
            [HarmonyPriority(Priority.First)]
            [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
            static void Postfix()
            {
                AssetBundleManager.Initialize();
                AssetBundleManager.Instance.LoadAllBundles(Path.Combine(Main.ModPath, "Bundles"));

                Prefabs.Initialize();
            }
        }
    }
}