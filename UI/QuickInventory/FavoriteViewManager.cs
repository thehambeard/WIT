using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using ModMaker;
using ModMaker.Utility;
using UnityEngine;
using QuickCast.Utilities;
using static QuickCast.Main;
using UnityEngine.UI;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Commands;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Controllers.Clicks;
using Kingmaker.Utility;
using Kingmaker.UI.UnitSettings;

namespace QuickCast.UI.QuickInventory
{
    public class FavoriteViewManager : MonoBehaviour, IModEventHandler, ISelectionHandler, IViewChangeHandler
    {
        public int Priority => 500;

        public static FavoriteViewManager CreateObject(UnitEntityData unit)
        {
            return null;
        }
        public void HandleModDisable()
        {
        }

        public void HandleModEnable()
        {
        }

        public void HandleViewChange()
        {
        }

        public void OnUnitSelectionAdd(UnitEntityData selected)
        {
        }

        public void OnUnitSelectionRemove(UnitEntityData selected)
        {
        }
    }
}
