using Kingmaker.UnitLogic;
using System.Collections.Generic;
using UnityEngine;

namespace QuickCast.UI.Monos.ViewControlGroup.MetaMagic
{
    internal class MetaMagicCtrlManager : MonoBehaviour
    {
        public VCGManager Owner { get; private set; }

        private RectTransform _content;
        private List<MetaButton> _buttons;
        private MetaButton _activeButton;

        private readonly int _maxMetas = 30;

        public void Initialize(VCGManager owner)
        {
            Owner = owner;

            _content = transform.Find("Content").GetComponent<RectTransform>();

            _buttons = new();
            for (int i = 0; i < _maxMetas; i++)
                _buttons.Add(Builders.BuildUI.BuildMetaButton(_content, this));
        }

        public void Fill(List<Feature> metas)
        {
            int index = 0;

            foreach (var m in metas)
            {
                _buttons[index].SetMetaMagic(m);

                if (!_buttons[index].gameObject.activeSelf)
                    _buttons[index].gameObject.SetActive(true);

                index++;
            }

            EmptyToEnd(index);
        }

        public void Empty()
        {
            EmptyToEnd(0);
        }

        public void EmptyToEnd(int index)
        {
            for (int i = index; i < _maxMetas; i++)
            {
                if (_buttons[i].gameObject.activeSelf)
                    _buttons[i].gameObject.SetActive(false);
            }
        }

        public void SetActiveButton(MetaButton button)
        {
            if (!_buttons.Contains(button))
                return;

            if (_activeButton != null)
                _activeButton.IsToggled = false;

            _activeButton = button;
        }
    }
}
