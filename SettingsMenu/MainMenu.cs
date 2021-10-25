using Kingmaker;
using ModMaker;
using UnityModManagerNet;
using QuickCast.Utilities;
using System.Linq;
using static QuickCast.Main;
using GL = UnityEngine.GUILayout;

namespace QuickCast.SettingsMenu
{
#if DEBUG
    internal class MainMenu : IModEventHandler, IMenuSelectablePage
    {
        public int Priority => 200;

        public string Name => "MainMenu";

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
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
            if (GL.Button("Test Spells"))
            {
                foreach(var unit in Game.Instance.Player.Party)
                {
                    Mod.Warning(unit.CharacterName);
                    Mod.Warning("Usable");
                    foreach(var ability in unit.Abilities)
                    {
                        Mod.Debug(ability);
                    }
                    Mod.Warning("Activatable");
                    foreach(var aa in unit.ActivatableAbilities)
                    {
                        Mod.Debug(aa);
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
#endif
}