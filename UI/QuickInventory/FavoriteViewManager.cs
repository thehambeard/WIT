using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using ModMaker;
using UnityEngine;

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
