using Kingmaker.UnitLogic;
using QuickCast.UI.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QuickCast.UI.Monos.ViewControlGroup.MetaMagic
{
    internal class MetaButton : ButtonWrapper
    {
        public MetaMagicCtrlManager Owner { get; private set; }

        private Feature _feat;
        
        public void Initialize(MetaMagicCtrlManager owner)
        {
            base.Initialize();

            Owner = owner;
        }

        public void SetMetaMagic(Feature feat)
        {
            _feat = feat;
            _image.sprite = _feat.Icon;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            Owner.SetActiveButton(this);
        }
    }
}
