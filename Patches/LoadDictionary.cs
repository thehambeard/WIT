/* Remove this file form your project if you are not going use the EAHelpers.cs*/
using Kingmaker.Blueprints;
using Kingmaker.Localization;
using WIT.Utilities;
using System;
using static WIT.Utilities.SettingsWrapper;
using Kingmaker.Blueprints.JsonSystem;

namespace WIT.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(BlueprintsCache), "Init")]
    static class BlueprintsCache_Init_Patch
    {
        static bool loaded = false;
        static void Postfix()
        {
            if (loaded) return;
            loaded = true;

            BundleManger.AddBundle("inventorytweaks");
        }
    }

    /*
    [HarmonyLib.HarmonyPatch(typeof(LibraryScriptableObject), "LoadDictionary")]
    [HarmonyLib.HarmonyPatch(typeof(LibraryScriptableObject), "LoadDictionary", new Type[0])]
    static class LibraryScriptableObject_LoadDictionary_Patch
    {
        static void Postfix(LibraryScriptableObject __instance)
        {
            var self = __instance;
            if (Main.Library != null) return;
            Main.Library = self;
            try
            {
#if DEBUG
                bool allow_guid_generation = true;
                
#else
                bool allow_guid_generation = false; //no guids should be ever generated in release
#endif
                Helpers.GuidStorage.load(Properties.Resources.blueprints, allow_guid_generation);

#if DEBUG
                string guid_file_name = $@"{ModPath}blueprints.txt";
                Helpers.GuidStorage.dump(guid_file_name);
#endif
                Helpers.GuidStorage.dump($@"{ModPath}loaded_blueprints.txt");

                BundleManger.AddBundle("inventorytweaks");
            }
            catch (Exception ex)
            {
                Main.Mod.Error(ex);
            }
        }
    }
    */
}
