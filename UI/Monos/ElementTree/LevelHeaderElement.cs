using System.Linq;

namespace QuickCast.UI.Monos.ElementTree
{
    internal class LevelHeaderElement : HeaderElement
    {
        public int Level;

        public override void SetElementLayout()
        {
            base.SetElementLayout();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override bool UpdateActive()
        {
            bool show = !IsHidden && (Parent == null ? true : Parent.IsExpanded) && (HasElements || ShowIfChildless) && (IsClaiming || ShowIfUnclaimed);

            if (show)
            {
                bool active = false;

                foreach (var spell in _children.Values.OfType<SpellElement>())
                {
                    if (spell != null && !spell.IsHiddenUnavailable)
                    {
                        active = true;
                        break;
                    }
                }
                show &= active;
            }

            if (gameObject.activeSelf != show)
                gameObject.SetActive(show);

            return show;
        }
    }
}
