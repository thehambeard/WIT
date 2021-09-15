using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModMaker;
using ModMaker.Utility;
using UnityModManagerNet;
using WIT.Utilities;
using GL = UnityEngine.GUILayout;
using static WIT.Main;

namespace WIT.SettingsMenu
{
    class MainMenu : IModEventHandler, IMenuSelectablePage
    {
        public int Priority => 200;

        public string Name => "MainMenu";
        
        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (GL.Button("Load bundle", GL.ExpandWidth(false)))
            {
                Mod.Core.UI.Clear();
                Mod.Core.CBUI.Clear();
                BundleManger.AddBundle("inventorytweaks");
                Mod.Core.UI.Update();
                Mod.Core.CBUI.Update();
                
            }
        }
        public void HandleModDisable() { }

        public void HandleModEnable() { }
    }
}
