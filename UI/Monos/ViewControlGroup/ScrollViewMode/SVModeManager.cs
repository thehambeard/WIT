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
            _toggleNWN = new(transform.FindChildRecursive("ToggleNWN"), ToggleNWN, "NWN2");
            _toggleLevelBook = new(transform.FindChildRecursive("ToggleLevelBook"), ToggleLevelBook, "Spellbook");
        }

        private void ToggleNWN()
        {
            
        }

        private void ToggleLevelBook()
        {
            _toggleLevelBook.IsPressed = !_toggleLevelBook.IsPressed;

            if (_toggleLevelBook.IsPressed)
                Owner.UpdateView(States.SortState.Book);
            else
                Owner.UpdateView(States.SortState.Level);
        }

        private void Settings()
        {

        }

        private void ToggleExpand()
        {

        }
        
    }
}
