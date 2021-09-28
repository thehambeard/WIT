﻿using Kingmaker;
using ModMaker;
using UnityModManagerNet;
using WIT.Utilities;
using System.Linq;
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
            if (GL.Button("Load bundle"))
            {
                Mod.Core.UI.Clear();
                Mod.Core.CBUI.Clear();
                Mod.Core.SpellVUI.Clear();
                AssetBundleManager.LoadAllBundles($"{SetWrap.ModPath}{Settings.BUNDLERELPATH}");
                Mod.Core.UI.Update();
                Mod.Core.CBUI.Update();
                Mod.Core.SpellVUI.Update();
            }
            if (GL.Button("Test Spells"))
            {
                foreach (var unit in Game.Instance.Player.Party)
                {
                    Mod.Error(unit.CharacterName);
                    int spellCount = 0;
                    foreach (var spellbook in unit.Spellbooks)
                    {
                        if (spellbook.Blueprint.Spontaneous)
                        {
                            for (int i = 0; i <= 10; i++)
                            {
                                spellbook.GetKnownSpells(i);
                                int count = spellbook.GetSpellSlotsCount(i);
                                spellCount += count;
                                Mod.Debug($"Spellbook level: {i} has {count} spells left to cast");
                            }
                        }
                    }
                }

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