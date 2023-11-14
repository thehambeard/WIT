using Owlcat.Runtime.Core.Utils;
using QuickCast.UI.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace QuickCast.UI.Monos.ViewControlGroup.ScrollViewMode
{
    internal class SVModeManager : MonoBehaviour
    {
        public VCGManager Owner { get; private set; }

        private ButtonWrapper _toggleNWN;
        private ButtonWrapper _toggleLevelBook;
        private ButtonWrapper _settings;
        private ButtonWrapper _toggleExpand;
        public void Initialize(VCGManager owner)
        {
            Owner = owner;
            _toggleNWN = transform.Find("Content/ToggleNWN/Button").gameObject.AddComponent<ButtonWrapper>();
            _toggleLevelBook = transform.Find("Content/ToggleLevelBook/Button").gameObject.AddComponent<ButtonWrapper>();
            _toggleLevelBook.OnLeftClickEvent.AddListener(ToggleLevelBook);
            _toggleLevelBook.DefaultText = "Level";
            _toggleLevelBook.PressedText = "Book";
            _toggleLevelBook.IsPressed = false;
            _toggleLevelBook.IsToggle = true;
            _toggleLevelBook.Initialize();
        }

        private void ToggleNWN()
        {
            
        }

        public void ToggleLevelBook() => SetSortState(!_toggleLevelBook.IsPressed ? States.SortState.Book : States.SortState.Level);

        public void SetSortState(States.SortState sortState)
        {
            Owner.UpdateView(sortState);
        }

        private void Settings()
        {

        }

        private void ToggleExpand()
        {

        }
        
    }
}
