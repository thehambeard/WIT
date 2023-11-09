using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuickCast.UI.Monos.ElementTree
{
    internal class HeaderElement : Element
    {
        protected TextMeshProUGUI _titleMesh;
        protected Button _expandButton;
        
        

        public string Title
        {
            get
            {
                if (_titleMesh == null)
                    _titleMesh = GetComponentInChildren<TextMeshProUGUI>();

                return _titleMesh?.text ?? null;
            }
            set
            {
                if (_titleMesh == null)
                    _titleMesh = GetComponentInChildren<TextMeshProUGUI>();

                if (_titleMesh != null)
                    _titleMesh.text = value;
            }
        }

        public override void Initialize()
        {
            _expandButton = GetComponentInChildren<Button>();
            _expandButton.onClick = new Button.ButtonClickedEvent();
            _expandButton.onClick.AddListener(OnExpandToggle);
        }
    }
}
