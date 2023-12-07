using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using QuickCast.UI.Monos.ViewControlGroup.MetaMagic;
using QuickCast.UI.Monos.ViewControlGroup.ScrollViewMode;
using QuickCast.UI.Monos.ViewControlGroup.SpellSV;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuickCast.UI.Monos.ViewControlGroup
{
    internal class VCGManager : MonoBehaviour,
        ISelectionHandler
    {
        public Dictionary<UnitEntityData, UnitVCGCollection> Units;
        private SVModeManager _svmModeManager;
        private MetaMagicCtrlManager _mmcManager;
        private SVManager _currentActive;
        private UnitEntityData _currentUnit;
        private States.SelectState _currentSelectState;

        public void Initialize()
        {
            EventBus.Subscribe(this);

            var contentWrapper = transform.Find("ContentWrapper");

            Units = new();

            _svmModeManager = Builders.BuildUI.BuildScrollViewMode(this);
            _mmcManager = Builders.BuildUI.BuildMetaMagicCtrl(this);

            foreach (var unit in Game.Instance.Player.PartyAndPets)
            {
                if (!Units.ContainsKey(unit))
                {
                    var collection = new UnitVCGCollection(unit);
                    Units.Add(unit, collection);
                    InitializeScrollView(contentWrapper, collection);
                }
            }

            _currentSelectState = States.SelectState.Spells;
        }

        public void UpdateView(UnitEntityData unit)
        {
            if (unit != _currentUnit)
                UpdateView(unit, _currentSelectState);
        }

        public void UpdateView(States.SelectState selectState)
        {
            if (selectState != _currentSelectState)
                UpdateView(_currentUnit, selectState);
        }

        public void UpdateView(States.SortState sortState)
        {
            if (_currentActive is SpellSVManager)
            {
                (_currentActive as SpellSVManager).SetSort(sortState);
            }
        }

        public void UpdateView(States.ShowUncastableState showUncastable)
        {
            if (_currentActive is SpellSVManager)
            {
                (_currentActive as SpellSVManager).SetShowUncastable(showUncastable);
            }
        }

        public void UpdateView(UnitEntityData unit, States.SelectState state)
        {
            if (_currentActive != null)
                _currentActive.gameObject.SetActive(false);

            switch (state)
            {
                case States.SelectState.Spells:
                    if (Units.ContainsKey(unit))
                    {
                        if (Units[unit].SpellScrollView.HasSpells)
                        {
                            _currentActive = Units[unit].SpellScrollView;
                            _currentActive.gameObject.SetActive(true);
                            _svmModeManager.SetSortState(Units[unit].SpellScrollView.SortState);
                            _svmModeManager.SetShowUncastableState(Units[unit].SpellScrollView.ShowUncastableState);
                            _mmcManager.Fill(Units[unit].UnitMetaMagic.Values.ToList());
                        }
                        else
                            SetViewNoAction();
                    }
                    break;
            }

            _currentSelectState = state;
            _currentUnit = unit;
        }

        public void SetViewMultiSelected()
        {
            if (_currentActive != null)
                _currentActive.gameObject.SetActive(false);

            _currentUnit = null;

            _currentActive = null;
        }

        public void SetViewNoAction()
        {
            if (_currentActive != null)
                _currentActive.gameObject.SetActive(false);

            _currentActive = null;
        }

        public void OnDestroy() => EventBus.Unsubscribe(this);

        public void OnUnitSelectionAdd(UnitEntityData selected)
        {
            if (Game.Instance.UI.SelectionManager.SelectedUnits.Count != 1)
            {
                SetViewMultiSelected();
                return;
            }

            UpdateView(selected);
        }

        public void OnUnitSelectionRemove(UnitEntityData selected) { }

        private void InitializeScrollView(Transform parent, UnitVCGCollection collection)
        {
            collection.SpellScrollView = Builders.BuildUI.BuildSpellScrollView(parent, collection.Unit);
            collection.SpellScrollView.Initialize();
        }
    }
}
