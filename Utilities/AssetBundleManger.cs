using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static QuickCast.Main;
using HarmonyLib;
using ModMaker;
using Kingmaker.PubSubSystem;

namespace QuickCast.Utilities
{
	public static class AssetBundleManager
	{
		public static Dictionary<string, AssetBundle> AssetBundles;
        public static Dictionary<string, GameObject> GameObjects;
        public static Dictionary<string, Sprite> SpriteObjects;

        public static void LoadAllBundles(string path)
        {
            Mod.Debug(MethodBase.GetCurrentMethod());
			AssetBundle asset;
            String fileName;

            if (AssetBundles == null)
                AssetBundles = new Dictionary<string, AssetBundle>();
            if (GameObjects == null)
                GameObjects = new Dictionary<string, GameObject>();
            if (SpriteObjects == null)
                SpriteObjects = new Dictionary<string, Sprite>();

            if (!Directory.Exists(path))
            {
                Mod.Error($"AssetBundle directory:{path}  not found");
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
                    foreach (var obj in asset.LoadAllAssets())
                    {
                        if (obj.GetType() == typeof(GameObject))
                        {
                            if (!GameObjects.ContainsKey(obj.name))
                            {
                                Mod.Log($"Loading: {obj.name}...");
                                GameObjects.Add(obj.name, (GameObject) obj);
                            }
                            else
                            {
                                Mod.Error($"Asset: {obj.name} already loaded.");
                            }
                        }
                        else if (obj.GetType() == typeof(Sprite))
                        {
                            if (!SpriteObjects.ContainsKey(obj.name))
                            {
                                Mod.Log($"Loading: {obj.name}...");
                                SpriteObjects.Add(obj.name, (Sprite)obj);
                            }
                            else
                            {
                                Mod.Error($"Sprite: {obj.name} already loaded.");
                            }
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
            Mod.Debug(MethodBase.GetCurrentMethod());
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
                
                GameObjects = null;
                AssetBundles = null;
            }
        }

        internal class ABMManager : IModEventHandler
        {
            public int Priority => 200;

            public void HandleModDisable()
            {
                Mod.Debug(MethodBase.GetCurrentMethod());
                UnloadAllBundles();
                EventBus.Unsubscribe(this);
            }

            public void HandleModEnable()
            {
                
            }
        }
    }
}