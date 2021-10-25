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

        public static bool RecalcPosScale
        {
            get => Mod.Settings.recalcPosScale;
            set => Mod.Settings.recalcPosScale = value;
        }
    }
}