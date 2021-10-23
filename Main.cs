using ModMaker;
using ModMaker.Utility;
using System;
using System.Reflection;
using UnityModManagerNet;
using static QuickCast.Utilities.SetWrap;

namespace QuickCast
{
#if (DEBUG)
    [EnableReloading]
#endif
    internal static class Main
    {
        public static LocalizationManager<DefaultLanguage> Local;
        public static ModManager<Core, Settings> Mod;
        public static MenuManager Menu;
        //internal static LibraryScriptableObject Library;

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            Local = new LocalizationManager<DefaultLanguage>();
            Mod = new ModManager<Core, Settings>();
            Menu = new MenuManager();
            modEntry.OnToggle = OnToggle;
#if (DEBUG)
            modEntry.OnUnload = Unload;
            return true;
        }

        private static bool Unload(UnityModManager.ModEntry modEntry)
        {
            Mod.Disable(modEntry, true);
            Menu = null;
            Mod = null;
            Local = null;
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
                Local.Enable(modEntry);
                Mod.Enable(modEntry, assembly);
                Menu.Enable(modEntry, assembly);
                Utilities.SetWrap.ModPath = modEntry.Path;
            }
            else
            {
                Menu.Disable(modEntry);
                Mod.Disable(modEntry, false);
                Local.Disable(modEntry);
                ReflectionCache.Clear();
            }
            return true;
        }

        internal static Exception Error(String message)
        {
            Mod.Error(message);
            return new InvalidOperationException(message);
        }
    }
}