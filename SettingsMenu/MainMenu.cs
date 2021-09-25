using Kingmaker;
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
                AssetBundleManager.LoadAllBundles($"{SetWrap.ModPath}{Settings.BUNDLERELPATH}");
                Mod.Core.UI.Update();
                Mod.Core.CBUI.Update();
            }
            if (GL.Button("Test Spells"))
            {
                foreach (var unit in Game.Instance.Player.Party)
                {
                    Mod.Error(unit.CharacterName);
                    foreach(var spellbook in unit.Spellbooks)
                    {
                        Mod.Debug($"|--{spellbook.Blueprint.Name}");
                        foreach (var spell in spellbook.GetAllMemorizedSpells().OrderBy(x => x.SpellLevel))
                        {
                            Mod.Error($"|--|--{spell.Spell.Name}");
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