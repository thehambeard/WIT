using ModMaker;
using UnityModManagerNet;
using WIT.Utilities;
using static WIT.Main;
using GL = UnityEngine.GUILayout;

namespace WIT.SettingsMenu
{
    internal class MainMenu : IModEventHandler, IMenuSelectablePage
    {
        public int Priority => 200;

        public string Name => "MainMenu";

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (GL.Button("Load bundle", GL.ExpandWidth(false)))
            {
                Mod.Core.UI.Clear();
                Mod.Core.CBUI.Clear();
                AssetBundleManager.LoadAllBundles(Settings.BUNDLERELPATH);
                Mod.Core.UI.Update();
                Mod.Core.CBUI.Update();
            }
        }

        public void HandleModDisable()
        {
        }

        public void HandleModEnable()
        {
        }
    }
}