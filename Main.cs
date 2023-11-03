using QuickCast.Utility;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace QuickCast
{
#if (DEBUG)
    [EnableReloading]
#endif
    internal static class Main
    {
        public static string ModPath;
        public static Utility.Logger Logger;
        public static ModEventHandler ModEventHandler;

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            ModPath = modEntry.Path;

            Logger = new(modEntry.Logger);
            ModEventHandler = new();

            modEntry.OnToggle = OnToggle;
#if (DEBUG)
            modEntry.OnGUI = OnGUIDebug;
            modEntry.OnUnload = Unload;
            return true;
        }

        private static bool Unload(UnityModManager.ModEntry modEntry)
        {
            ModEventHandler.Disable(modEntry, true);
            return true;
        }

#else
            return true;
        }
#endif

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (value)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                ModEventHandler.Enable(modEntry, assembly);
            }
            else
            {
                ModEventHandler.Disable(modEntry);
            }

            return true;
        }

        private static void OnGUIDebug(UnityModManager.ModEntry modEntry)
        {
            if (GUILayout.Button("Load All"))
            {
                if (AssetBundleManager.Instance == null)
                    AssetBundleManager.Initialize();

                AssetBundleManager.Instance.LoadAllBundles(Path.Combine(ModPath, "Bundles"));
            }
            if (GUILayout.Button("Window"))
            {
                //MainUIManager.CreateObject();
            }
        }
    }
}