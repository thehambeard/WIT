using ModMaker.Utility;
using QuickCast.UI.QuickInventory;
using System.Collections.Generic;
using UnityEngine;
using static QuickCast.Main;

namespace QuickCast.Utilities
{
    public static class SetWrap
    {
        //public const string HOTKEY_TOGGLE_MINMAX = 
        public static string LocalizationFileName
        {
            get => Mod.Settings.localizationFileName;
            set => Mod.Settings.localizationFileName = value;
        }

        public static Vector3 Window_Pos
        {
            get => Mod.Settings.window_pos;
            set => Mod.Settings.window_pos = value;
        }

        public static Vector3 Window_Scale
        {
            get => Mod.Settings.window_scale;
            set => Mod.Settings.window_scale = value;
        }
        public static float RefreshRate
        {
            get => Mod.Settings.delayInMilliSeconds;
            set => Mod.Settings.delayInMilliSeconds = value;
        }

        public static SerializableDictionary<MainWindowManager.ViewPortType, List<bool>> HeaderStates
        {
            get => Mod.Settings.header_states;
            set => Mod.Settings.header_states = value;
        }
    }
}