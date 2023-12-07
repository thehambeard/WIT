using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.FactLogic;
using QuickCast.UI.Monos.ViewControlGroup.SpellSV;
using System.Collections.Generic;

namespace QuickCast.UI.Monos.ViewControlGroup
{
    internal class UnitVCGCollection
    {
        public readonly UnitEntityData Unit;
        public SortedList<string, Feature> UnitMetaMagic { get; private set; }

        public SpellSVManager SpellScrollView;

        public UnitVCGCollection(UnitEntityData unit)
        {
            this.Unit = unit;
            InitializeUnitMetaMagic();
        }

        public void InitializeUnitMetaMagic()
        {
            UnitMetaMagic = new();

            foreach (var feat in Unit.Facts.GetAll<Feature>())
            {
                if (feat.GetComponent<AddMetamagicFeat>() != null)
                {
                    UnitMetaMagic.Add(feat.Blueprint.name, feat);
                }
            }
        }
    }
}
