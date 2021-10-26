﻿using Kingmaker;
using ModMaker;
using UnityModManagerNet;
using QuickCast.Utilities;
using System.Linq;
using static QuickCast.Main;
using GL = UnityEngine.GUILayout;
using Kingmaker.UI.Common;

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