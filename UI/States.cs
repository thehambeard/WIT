namespace QuickCast.UI
{
    internal class States
    {
        public enum WindowState
        {
            None,
            Maximized,
            Minimized
        }

        public enum SelectState
        {
            None,
            Spells,
            Abilities,
            Favorites,
            Scrolls,
            Wands,
            Potions
        }

        public enum SpellState
        {
            None,
            Standard,
            NWN
        }

        public enum SortState
        {
            None,
            Level,
            Book
        }
    }
}
