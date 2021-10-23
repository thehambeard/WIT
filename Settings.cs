using UnityEngine;
using UnityModManagerNet;

namespace QuickCast
{
    public class Settings : UnityModManager.ModSettings
    {
        //settings go here
        public string lastModVersion;

        public string localizationFileName;
        public string modPath;
        public bool containScrolls;
        public bool containWands;
        public bool containPotions;
        public const string BUNDLERELPATH = "Bundles";
        public Vector3 window_pos;
        public Vector3 window_scale;

    }
}