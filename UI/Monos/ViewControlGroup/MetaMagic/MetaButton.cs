using Kingmaker.UnitLogic;
using QuickCast.UI.Utility;
using UnityEngine.EventSystems;

namespace QuickCast.UI.Monos.ViewControlGroup.MetaMagic
{
    internal class MetaButton : QCButton
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
            DefaultSprite = _feat.Icon;
            _image.sprite = _feat.Icon;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            Owner.SetActiveButton(this);
        }
    }
}
