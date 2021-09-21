using Kingmaker;
using Kingmaker.UI;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.UI.Tooltip;
using System;
using UnityEngine;
using UnityEngine.UI;
using WIT.Utilities;
using static WIT.Main;
using static WIT.Utilities.SetWrap;
namespace WIT.UI.ContainerBar
{
    internal class CBUIManager : MonoBehaviour
    {
        private Toggle scrollTog, wandTog, potionTog;

        public static CBUIManager CreateObject()
        {
            try
            {
                var kmInventory = (RectTransform)Game.Instance.UI.Common.transform?.Find("ServiceWindow/Inventory/Stash") ?? throw new NullReferenceException("kmInventory");

                GameObject containerBar = new GameObject();
                containerBar.name = "ContainerBar";

                var glg = containerBar.AddComponent<GridLayoutGroupWorkaround>();
                var csf = containerBar.AddComponent<ContentSizeFitter>();
                glg.cellSize = new Vector2(48f, 48f);
                glg.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                glg.constraintCount = 1;
                glg.spacing = new Vector2(1f, 0f);
                glg.startAxis = GridLayoutGroup.Axis.Horizontal;
                glg.startCorner = GridLayoutGroup.Corner.UpperLeft;
                glg.DoWorkaround = true;

                csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

                var kmToggleButton = (RectTransform)Game.Instance.UI.Common.transform?.Find("ServiceWindow/Inventory/Stash/Filters/SwitchBar/All") ?? throw new NullReferenceException("kmToggleButton");

                var newToggleButton = GameObject.Instantiate(kmToggleButton);
                var ntbt = newToggleButton.GetComponent<Toggle>();
                DestroyImmediate(newToggleButton.GetComponent<TooltipTrigger>());
                ntbt.group = null;

                //void SetToggle(string s, Sprite icon)
                //{
                //    var r = GameObject.Instantiate(newToggleButton.gameObject);
                //    r.name = s;
                //    r.transform.Find("Icon").gameObject.GetComponent<Image>().sprite = icon;
                //    r.transform.SetParent(containerBar.transform, false);
                //}

                //SetToggle("Scroll", BundleManger.LoadedSprites["Scoll_Container_Icon"]);
                //SetToggle("Potion", BundleManger.LoadedSprites["Potion_Container_Icon"]);
                //SetToggle("Wand", BundleManger.LoadedSprites["Wand_Container_Icon"]);

                containerBar.transform.SetParent(kmInventory, false);
                var tmp = containerBar.transform.localPosition;
                containerBar.transform.localPosition = new Vector3(-596.6f, 420.0f);

                return containerBar.gameObject.AddComponent<CBUIManager>();
            }
            catch (Exception ex)
            {
                Mod.Error("UI creation failed at: " + ex.Message + ex.StackTrace);
            }
            return new CBUIManager();
        }

        private void HandleScrollToggle(bool b)
        {
            ContainScrolls = b;
            Game.Instance.UI.Common.transform.Find("ServiceWindow/Inventory/Stash").GetComponent<Stash>().Filter.ApplySortAndFilters();
        }

        private void HandlePotionToggle(bool b)
        {
            ContainPotions = b;
            Game.Instance.UI.Common.transform.Find("ServiceWindow/Inventory/Stash").GetComponent<Stash>().Filter.ApplySortAndFilters();
        }

        private void HandleWandToggle(bool b)
        {
            ContainWands = b;
            Game.Instance.UI.Common.transform.Find("ServiceWindow/Inventory/Stash").GetComponent<Stash>().Filter.ApplySortAndFilters();
        }

        private void Awake()
        {
            try
            {
                scrollTog = transform.Find("Scroll").GetComponent<Toggle>();
                wandTog = transform.Find("Wand").GetComponent<Toggle>();
                potionTog = transform.Find("Potion").GetComponent<Toggle>();

                scrollTog.isOn = ContainScrolls;
                scrollTog.onValueChanged = new Toggle.ToggleEvent();
                scrollTog.onValueChanged.AddListener(HandleScrollToggle);
                wandTog.isOn = ContainWands;
                wandTog.onValueChanged = new Toggle.ToggleEvent();
                wandTog.onValueChanged.AddListener(HandleWandToggle);
                potionTog.isOn = ContainPotions;
                potionTog.onValueChanged = new Toggle.ToggleEvent();
                potionTog.onValueChanged.AddListener(HandlePotionToggle);
            }
            catch (Exception ex)
            {
                Mod.Error(ex.Message + ex.StackTrace);
            }
        }

        private void Update()
        {
        }
    }
}