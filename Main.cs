using System;
using System.Reflection;
using UnityModManagerNet;

namespace QuickCast
{
#if (DEBUG)
    [EnableReloading]
#endif
    internal static class Main
    {
        public static string ModPath;

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            ModPath = modEntry.Path;
            modEntry.OnToggle = OnToggle;
#if (DEBUG)
            modEntry.OnUnload = Unload;
            return true;
        }

        private static bool Unload(UnityModManager.ModEntry modEntry)
        {
            return true;
        }

#else
            return true;
        }
#endif

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            
            return true;
        }
    }
}