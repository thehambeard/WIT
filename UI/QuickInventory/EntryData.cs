using Kingmaker.Items;
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
        public string EntryText;
        public MechanicActionBarSlot MSlot { get; set; }
        public Transform Transform { get; set; }
        public Button Button { get; set; }
        public TextMeshProUGUI UsesText { get; set; }
        public TextMeshProUGUI DCText { get; set; }
    }
}
