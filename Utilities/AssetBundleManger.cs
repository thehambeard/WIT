using ModMaker;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static QuickCast.Main;

namespace QuickCast.Utilities
{
    public static class AssetBundleManager
    {
        public static Dictionary<string, AssetBundle> AssetBundles;
        public static Dictionary<string, GameObject> GameObjects;

        public static void LoadAllBundles(string path)
        {
            AssetBundle asset;
            string fileName;

            if (AssetBundles == null)
                AssetBundles = new Dictionary<string, AssetBundle>();
            if (GameObjects == null)
                GameObjects = new Dictionary<string, GameObject>();

            path = $"{ModPath}{path}";
            Mod.Debug(path);
            if (!Directory.Exists(path))
            {
                Mod.Error("AssetBundle directory not found");
                return;
            }

            string[] fileNames = (from file in Directory.GetFiles(path)
                                  where !file.EndsWith("manifest")
                                  where Path.GetFileName(file) != Path.GetFileName(path)
                                  where Path.GetFileName(file) != "AssetBundles"
                                  select file).ToArray<string>();
            foreach (string file in fileNames)
            {
                fileName = Path.GetFileName(file);
                if (!AssetBundles.ContainsKey(fileName))
                {
                    asset = AssetBundle.LoadFromFile(file);
                    if (asset == null)
                    {
                        Mod.Error($"AssetBundle: {fileName} failed to load");
                        return;
                    }
                    AssetBundles.Add(fileName, asset);
                    foreach (var obj in asset.LoadAllAssets<GameObject>())
                    {
                        if (!GameObjects.ContainsKey(obj.name))
                        {
                            Mod.Log($"Loading: {obj.name}...");
                            GameObjects.Add(obj.name, obj);
                        }
                        else
                        {
                            Mod.Error($"Asset: {obj.name} already loaded.");
                        }
                    }
                }
                else
                {
                    Mod.Error($"AssetBundle: {fileName} already loaded.");
                }
            }
        }

        public static void UnloadAllBundles()
        {
            if (AssetBundles != null)
            {
                foreach (KeyValuePair<string, AssetBundle> kvp in AssetBundles)
                {
                    Mod.Log($"Unloading asset bundle: {kvp.Key}");
                    if (kvp.Value != null)
                        kvp.Value.Unload(true);
                }
                if (GameObjects != null)
                    GameObjects.Clear();

                //AssetBundles.Clear();
                GameObjects = null;
                AssetBundles = null;
            }
        }

        internal class ABMManager : IModEventHandler
        {
            public int Priority => 100;

            public void HandleModDisable()
            {
                UnloadAllBundles();
                //EventBus.Unsubscribe(this);
            }

            public void HandleModEnable()
            {
                //EventBus.Subscribe(this);
                LoadAllBundles(Settings.BUNDLEPATH);
            }
        }
    }
}