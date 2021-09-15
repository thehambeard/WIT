using UnityModManagerNet;

namespace WIT
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
    }
}