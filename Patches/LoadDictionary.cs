/* Remove this file form your project if you are not going use the EAHelpers.cs*/

using Kingmaker.Blueprints.JsonSystem;
using WIT.Utilities;
using static WIT.Main;

namespace WIT.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(BlueprintsCache), "Init")]
    internal static class BlueprintsCache_Init_Patch
    {
        private static bool loaded = false;

        private static void Postfix()
        {
            if (loaded) return;
            loaded = true;

            var path = $"{SetWrap.ModPath}{Settings.BUNDLERELPATH}";
            Mod.Debug($"Loading All Bundles at {path}");
            AssetBundleManager.LoadAllBundles(path);
        }
    }
}