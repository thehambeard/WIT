using JetBrains.Annotations;
using Kingmaker.PubSubSystem;
using Kingmaker.View;
using ModMaker;
using System;
using System.Reflection;
using static WIT.Main;

namespace WIT.Utilities
{
    public class EventTest : IModEventHandler
    {
        public int Priority => 200;

        public void HandleHighlightChange([NotNull] UnitEntityView unit)
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
        }

        public void HandleModDisable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            EventBus.Unsubscribe(this);
        }

        public void HandleModEnable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            EventBus.Subscribe(this);
        }

        public void HandleSwitchSelectionUnitInGroup()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public void OnAreaBeginUnloading()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
        }

        public void OnAreaDidLoad()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
        }
    }
}