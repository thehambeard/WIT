using Kingmaker;
using Kingmaker.Items;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.Constructor;
using Kingmaker.UnitLogic.Abilities;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WIT.UI.QuickInventory
{
    public class ItemButtonManager : MonoBehaviour
    {
        private ButtonPF _button;
        private TextMeshProUGUI _label;
        private RectTransform _buttonRect;
        private float _scale;
        public int Index { get; set; }
        public object Data { get; set; }
        public int Count { get; set; }

        public event Func<AbilityData, bool> OnClick;

        private string _text;
        private float _width;

        public static ItemButtonManager CreateObject(GameObject source)
        {
            GameObject button = GameObject.Instantiate(source);
            button.transform.SetParent(null, false);
            return button.AddComponent<ItemButtonManager>();
        }

        private void Start()
        {
            try
            {
                _buttonRect = (RectTransform)gameObject.transform;
                _button = gameObject.GetComponent<ButtonPF>();
                _button.onClick = new Button.ButtonClickedEvent();
                _button.onClick.AddListener(new UnityAction(HandleOnClick));
                var text = gameObject.GetComponentInChildren<TextMeshProUGUI>();

                if (Data != null && Data is AbilityData)
                    text.text = ((AbilityData)Data).Name;
                else if (Data != null && Data is ItemEntity)
                    text.text = ((ItemEntity)Data).Name;
            }
            catch (Exception ex)
            {
                Main.Mod.Debug(ex.Message + ex.StackTrace);
            }
            EventBus.Subscribe(this);
        }

        private void FixMutlipleSelected()
        {
            int multi = 0;
            var ginst = Game.Instance;
            var sman = ginst.UI.SelectionManager;
            foreach (var unit in ginst.Player.AllCharacters)
            {
                if (sman.IsSelected(unit)) multi++;
            }
            if (multi == 0 || multi > 1) sman.SwitchSelectionUnitInGroup(ginst.Player.MainCharacter);
        }

        private void HandleOnClick()
        {
            //FixMutlipleSelected();
            //var wielder = UIUtility.GetCurrentCharacter().Descriptor;

            //Item.OnDidEquipped(wielder);
            //if (Item.Ability.Data.TargetAnchor != AbilityTargetAnchor.Owner)
            //{
            //    Game.Instance.SelectedAbilityHandler.SetAbility(Item.Ability.Data);
            //}
            //else
            //{
            //    UIUtility.GetCurrentCharacter().Commands.Run(new UnitUseAbility(Item.Ability.Data, UIUtility.GetCurrentCharacter()));
            //}
        }

        private void Update()
        {
        }
    }
}