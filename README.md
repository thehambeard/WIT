# WIT
 Pathfinder: WotR Inventory Tweak


This is the alpha release of Quick Cast (formally known as Inventory Tweaks in Kingmaker).
this mod is Save Game safe, install/uninstall will not break your saves.
this is an alpha release, expect there to be bugs.  The amount of abilities and spells one can have seems to be a lot higher than kingmaker.  This alpha release is to help me test the mod from start to endgame for bugs and performance issues.
Tested compatible from 1.1.0d beta to 1.1.0i versions of the game.


How to use:
* Install the mod using Unity Mod Manager
* The first time the mod is ran the UI might be in an odd location.
* All four edges of the window can be clicked and dragged to move the window where you would like it.
* The bottom corner jewel can be clicked and dragged to scale the window to your liking as well.
* These settings will be remembered going forward.
* The upper left jewel can be clicked to minimize the UI window.  Click the box it minimizes to to restore the window.  The minimized box can be moved with click dragging as well.
* The upper right jewel will expand / contract all the items in the current view port
* The lower left jewel will be for settings, this area has not been implemented yet.
* The lower right jewel will scale the window by click dragging.
* There are 6 viewports, Spells, Scrolls, Potions, Wands, Special, Favorite
  * Spells shows you the current characters available spells. As you use the spells they will be removed from the list.  If there are no available spells then the view port will say so. The first number within the circle is the DC for the spell, the second is how many more times you can cast the spell for the day.
  * Scrolls, Potions, Wands shows any of these items that are in the inventory.  Clicking on them will use the item.  Items will always be used by the current character. The first number within the circle is how many of the item you have stacked in your inventory.  Any unstacked items, such as partially used wands will be shown in a separate entry. The second number is the amount of charges remaining.  Potions and scroll always have 1 charge.  A note on belt slots.  The mod uses belt slot 5 to handle the items being used.  If you have anything in belt slot 5 it will be removed back to your inventory.  After using a wand if there are charges remaining it will stay in belt slot 5 till something else is used or the charges are exhausted.
  * Special shows abilities the current character has, including Activatable Abilities.  Under the abilities tab, it will show usable abilities.  You may see items listed here if you have those items in a belt slot.  This is normal behavior due to how the belt slots work in game. The first number within the circle is again the DC when using this ability. The second number is how many uses are left on the ability.  Under the Activatable abilities tab, it will show abilities that can toggled or activated, such as Fighting Defensively.  The first circle will have an X in it if it active. The second square will have how many more times the ability can be activated for the day.
  * Favorites is not implemented yet, but it will hold any spell, item, ability that you deem to have in one place making these commonly used entries even easier.
* To use any entry simply click on it and it will automatically use the item if the target is self, or it'll bring up the target selector UI if the entry can be used on others.
* To Hide/Show a list of entries under the purple header, click on the purple header and it will either expand or collapse all of the entries.  Remembering the status of these headers is planned for a future release.  As of now the status will be reset each new scene that the player visits.

### Known issues (I will try to keep this as current as possible):
* A few minor graphical glitches that will be ironed out as I go.

### FOR BUGS USE THE ISSUE TRACKER, DO NOT USE THE ISSUE TRACKER TO MAKE SUGGESTIONS!
### SUGGESTIONS CAN BE MADE VIA DISCORD #mod-user-general (@MENTION ME)
### DO NOT DM ME WITH SUGGESTIONS.
### ANYONE NOT ADHERING TO THOSE SIMPLE RULES WILL BE IGNORED OR REPORTED FOR REPEAT SEVERE ABUSERS.
