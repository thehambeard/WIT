using QuickCast.UI.Utility;
using UnityEngine;

namespace QuickCast.UI.Monos.ViewControlGroup.ScrollViewMode
{
    internal class SVModeManager : MonoBehaviour
    {
        public VCGManager Owner { get; private set; }

        private QCButton _toggleNWN;
        private QCButton _toggleShowUncastable;
        private QCButton _toggleLevelBook;
        private QCButton _settings;
        private QCButton _toggleExpand;
        public void Initialize(VCGManager owner)
        {
            Owner = owner;
            _toggleNWN = transform.Find("Content/ToggleNWN/Button").gameObject.AddComponent<QCButton>();

            _toggleLevelBook = Builders.BuildUI.BuildQCButton<QCButton>(
                button: transform.Find("Content/ToggleLevelBook/Button"),
                onLeftClick: ToggleLevelBook,
                toggable: true,
                defaultText: "Book",
                toggleText: "Level");

            _toggleShowUncastable = Builders.BuildUI.BuildQCButton<QCButton>(
                button: transform.Find("Content/ToggleShowUncastable/Button"),
                onLeftClick: ToggleShowUncastable,
                toggable: true,
                defaultText: "Hid",
                toggleText: "Shown");

        }

        private void ToggleNWN()
        {

        }

        public void SetShowUncastableState(States.ShowUncastableState showUncastableState)
        {
            _toggleShowUncastable.IsToggled = showUncastableState == States.ShowUncastableState.Shown;
        }

        public void SetSortState(States.SortState sortState)
        {
            _toggleLevelBook.IsToggled = sortState == States.SortState.Level;
        }

        private void ToggleShowUncastable() => Owner.UpdateView(_toggleShowUncastable.IsToggled ? States.ShowUncastableState.Shown : States.ShowUncastableState.Hid);

        public void ToggleLevelBook() => Owner.UpdateView(_toggleLevelBook.IsToggled ? States.SortState.Level : States.SortState.Book);

        private void Settings()
        {

        }

        private void ToggleExpand()
        {

        }

    }
}
