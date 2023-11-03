using Kingmaker;
using Kingmaker.UI.MVVM;
using UnityEngine.SceneManagement;

namespace QuickCast.Utility
{
    internal class Helpers
    {
        public static bool IsInMainMenu() => !Game.HasInstance || !Game.Instance.AlreadyInitialized || RootUIContext.Instance.IsMainMenu || SceneManager.GetSceneByName("MainMenu").isLoaded;
    }
}
