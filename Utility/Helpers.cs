using Kingmaker;
using Kingmaker.UI.MVVM;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuickCast.Utility
{
    internal class Helpers
    {
        public static bool IsInMainMenu() => !Game.HasInstance || !Game.Instance.AlreadyInitialized || RootUIContext.Instance.IsMainMenu || SceneManager.GetSceneByName("MainMenu").isLoaded;


        public static Vector3 MapValueVector(float a0, float a1, float b0, float b1, float a)
        {
            float v = b0 + (b1 - b0) * ((a - a0) / (a1 - a0));
            return new Vector3(v, v, v);
        }
    }
}
