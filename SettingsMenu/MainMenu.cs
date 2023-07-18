using Kingmaker.PubSubSystem;
using ModMaker;
using QuickCast.Utilities;
using UnityModManagerNet;
using static QuickCast.Main;
using GL = UnityEngine.GUILayout;

namespace QuickCast.SettingsMenu
{

    internal class MainMenu : IModEventHandler, IMenuSelectablePage
    {
        public int Priority => 200;

        public string Name => "MainMenu";

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (GL.Button("Reset Window"))
            {
                SetWrap.Window_Pos = default;
                SetWrap.Window_Scale = new UnityEngine.Vector3(1f, 1f, 1f);
                Mod.Core.UI.MainWindowManager.IsDirty = true;
            }
            {
#if DEBUG
                if (GL.Button("Load bundle"))
                {
                    Mod.Core.UI.Clear();
                    Mod.Core.SpellVUI.Clear();
                    Mod.Core.ItemVUI.Clear();
                    Mod.Core.SpecialVUI.Clear();
                    Mod.Core.FavoriteVUI.Clear();
                    AssetBundleManager.LoadAllBundles(Settings.BUNDLEPATH);
                    Mod.Core.UI.Update();
                    Mod.Core.SpellVUI.Update();
                    Mod.Core.ItemVUI.Update();
                    Mod.Core.SpecialVUI.Update();
                    Mod.Core.FavoriteVUI.Update();
                }

                if(GL.Button("List Subscribers"))
                {
                    foreach (var e in EventBus.GlobalSubscribers.m_Listeners)
                    {
                        if (e.Key.FullName.Contains("QuickCast"))
                        {
                            foreach (var s in e.Value.List)
                                Main.Mod.Debug(s.ToString());
                        }
                    }
                }
#endif
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