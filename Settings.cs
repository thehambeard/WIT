using ModMaker.Utility;
using QuickCast.UI.QuickInventory;
using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;

namespace QuickCast
{
    public class Settings : UnityModManager.ModSettings
    {
        //settings go here
        public string lastModVersion;
        public string localizationFileName;
        public const string BUNDLEPATH = "Bundles";
        public float delayInMilliSeconds = 750f;
        public Vector3 window_pos;
        public Vector3 window_scale;
        public SerializableDictionary<MainWindowManager.ViewPortType, List<bool>> header_states;
    }
}