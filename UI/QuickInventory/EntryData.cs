﻿using Kingmaker.Items;
using Kingmaker.UI.UnitSettings;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.ActivatableAbilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuickCast.UI.QuickInventory
{

    public class EntryData
    { 
        public Transform Transform { get; set; }
        public Button Button { get; set; }
        public TextMeshProUGUI UsesText { get; set; }
        public TextMeshProUGUI DCText { get; set; }
    }

    public class SpellEntryData : EntryData
    {
        public AbilityData Data { get; set; }
    }

    public class ItemEntryData : EntryData
    {
        public ItemEntity Data { get; set; }
    }

    public class AbilityEntryData : EntryData
    {
        public MechanicActionBarSlotAbility Slot { get; set; }
    }

    public class ActivatableEntryData : EntryData
    {
        public MechanicActionBarSlotActivableAbility Slot { get; set; }
    }
}