using UnityEngine;
using static QuickCast.Main;

namespace QuickCast.Utilities
{
    public static class SetWrap
    {
        public static string LocalizationFileName
        {
            get => Mod.Settings.localizationFileName;
            set => Mod.Settings.localizationFileName = value;
        }

        public static string ModPath
        {
            get => Mod.Settings.modPath;
            set => Mod.Settings.modPath = value;
        }

        public static bool ContainWands
        {
            get => Mod.Settings.containWands;
            set => Mod.Settings.containWands = value;
        }

        public static bool ContainPotions
        {
            get => Mod.Settings.containPotions;
            set => Mod.Settings.containPotions = value;
        }

        public static bool ContainScrolls
        {
            get => Mod.Settings.containScrolls;
            set => Mod.Settings.containScrolls = value;
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
    }
}