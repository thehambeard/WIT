using Kingmaker.PubSubSystem;
using ModMaker;
using System;
using static WIT.Main;
using static WIT.Utilities.SetWrap;

namespace WIT
{
    internal class Core : IModEventHandler
    {
        public UI.QuickInventory.UIController UI { get; internal set; }
        public UI.ContainerBar.CBUIController CBUI { get; internal set; }
        public Utilities.EventTest EventTest { get; internal set; }
        public int Priority => 200;

        public void ResetSettings()
        {
            Mod.ResetSettings();
            Mod.Settings.lastModVersion = Mod.Version.ToString();
            LocalizationFileName = Local.FileName;
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
            EventTest = null;
            EventBus.Unsubscribe(this);
        }
    }
}