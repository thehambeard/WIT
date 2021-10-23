using Kingmaker.PubSubSystem;
using ModMaker;
using System;
using static QuickCast.Main;
using static QuickCast.Utilities.SetWrap;
using UnityEngine;

namespace QuickCast
{
    internal class Core : IModEventHandler
    {
        public Controllers.QuickInvUIController UI { get; internal set; }
        public Controllers.SpellViewController SpellVUI { get; internal set; }
        public Controllers.ItemViewController ItemVUI { get; internal set; }
        public int Priority => 200;

        public void ResetSettings()
        {
            Mod.ResetSettings();
            Mod.Settings.lastModVersion = Mod.Version.ToString();
            LocalizationFileName = Local.FileName;
            Mod.Settings.window_scale = new Vector3(.7f, .7f);
            Mod.Settings.window_pos = new Vector3(Screen.height * .5f, Screen.width * .5f);
        }

        public void HandleModEnable()
        {
            if (!string.IsNullOrEmpty(LocalizationFileName))
            {
                Local.Import(LocalizationFileName, e => Mod.Error(e));
                LocalizationFileName = Local.FileName;
            }
            if (!Version.TryParse(Mod.Settings.lastModVersion, out Version version) || version < new Version(0, 0, 0))
                ResetSettings();
            else
            {
                Mod.Settings.lastModVersion = Mod.Version.ToString();
            }
            EventBus.Subscribe(this);
        }

        public void HandleModDisable()
        {
            EventBus.Unsubscribe(this);
        }
    }
}