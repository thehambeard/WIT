using Kingmaker.PubSubSystem;
using ModMaker;
using System;
using static QuickCast.Main;
using static QuickCast.Utilities.SetWrap;
using UnityEngine;
using System.Reflection;
using QuickCast.Utilities;

namespace QuickCast
{
    internal class Core : IModEventHandler
    {
        public Controllers.QuickInvUIController UI { get; internal set; }
        public Controllers.SpellViewController SpellVUI { get; internal set; }
        public Controllers.ItemViewController ItemVUI { get; internal set; }
        public Controllers.FavoriteViewController FavoriteVUI { get; internal set; }
        public Controllers.SpecialViewController SpecialVUI { get; internal set; }
        public int Priority => 200;

        public void ResetSettings()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            Mod.ResetSettings();
            Mod.Settings.lastModVersion = Mod.Version.ToString();
            LocalizationFileName = Local.FileName;
            SetWrap.Window_Pos = new Vector3(1f, 1f, 1f);
            SetWrap.Window_Scale = new Vector3(.6f, .6f, .6f);
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