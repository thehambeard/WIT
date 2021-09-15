using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModMaker;
using ModMaker.Utility;
using Kingmaker;
using Kingmaker.PubSubSystem;
using static WIT.Main;
using System.Reflection;
using Kingmaker.EntitySystem.Entities;
using JetBrains.Annotations;
using Kingmaker.View;

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
