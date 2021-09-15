﻿using Kingmaker.PubSubSystem;
using ModMaker;
using ModMaker.Utility;
using System.Reflection;
using UnityEngine;
using Kingmaker;
using static WIT.Main;
using System;

namespace WIT.UI.ContainerBar
{
    class CBUIController : IModEventHandler, IAreaHandler
    {
        public int Priority => 400;

        public CBUIManager ContainersUI { get; private set; }

        public void Attach()
        {
            
            //if (ContainersUI == null)
            //    ContainersUI = CBUIManager.CreateObject();
        }

        public void Detach()
        {
            
            ContainersUI.SafeDestroy();
            ContainersUI = null;
        }

        public void Update()
        {
            
            Detach();
            Attach();
        }

#if DEBUG
        public void Clear()
        {
            
            Transform quickInventory;
            while (quickInventory = Game.Instance.UI.Common.transform.Find("ServiceWindow/Inventory/Stash/ContainerBar"))
            {
                quickInventory.SafeDestroy();
            }
            quickInventory = null;
        }
#endif

        public void HandleModEnable()
        {
            

            Mod.Core.CBUI = this;
            Attach();

            EventBus.Subscribe(this);
        }

        public void HandleModDisable()
        {
            

            EventBus.Unsubscribe(this);
            Detach();
            Mod.Core.UI = null;
        }

        public void OnAreaBeginUnloading() { }
        
        public void OnAreaDidLoad() { Attach(); }
    }
}