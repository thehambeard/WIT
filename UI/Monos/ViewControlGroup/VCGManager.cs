using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using QuickCast.UI.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Kingmaker.PubSubSystem;
using QuickCast.UI.Monos.ViewControlGroup.SpellSV;
using QuickCast.UI.Monos.ViewControlGroup.ScrollViewMode;

namespace QuickCast.UI.Monos.ViewControlGroup
{
    internal class VCGManager : MonoBehaviour,
        ISelectionHandler
    {
        public Dictionary<UnitEntityData, SpellSVManager> SpellSVs;

        private SVModeManager _svmManager;
        private Component _currentActive;
        private UnitEntityData _currentUnit;
        private States.SelectState _currentSelectState;

        public void Initialize()
        {
            EventBus.Subscribe(this);

            var contentWrapper = transform.Find("ContentWrapper");

            SpellSVs = new();

            _svmManager = Builders.BuildUI.BuildScrollViewMode(this);

            foreach (var unit in Game.Instance.Player.PartyAndPets)
            {
                InitializeScrollView(contentWrapper, unit);
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
            if(selectState != _currentSelectState)
                UpdateView(_currentUnit, selectState);
        }

        public void UpdateView(States.SortState sortState)
        {
            if (_currentActive is SpellSVManager)
            {
                (_currentActive as SpellSVManager).SetSort(sortState);
            }
        }

        public void UpdateView(UnitEntityData unit, States.SelectState state) 
        {
            if (_currentActive != null)
                _currentActive.gameObject.SetActive(false);

            switch (state)
            {
                case States.SelectState.Spells:
                    if (SpellSVs.ContainsKey(unit))
                    {
                        if (SpellSVs[unit].HasSpells)
                        {
                            _currentActive = SpellSVs[unit];
                            _currentActive.gameObject.SetActive(true);
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

        private void InitializeScrollView(Transform parent, UnitEntityData unit)
        {
            if (SpellSVs == null)
                SpellSVs = new();

            if (!SpellSVs.ContainsKey(unit))
            {
                var ssv = Builders.BuildUI.BuildSpellScrollView(parent, unit);
                ssv.Initialize();
                SpellSVs.Add(unit, ssv);
            }
        }
    }
}
