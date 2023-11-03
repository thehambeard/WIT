using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace QuickCast.Utility
{
    internal class AssetBundleManager
    {
        public static AssetBundleManager Instance { get; private set; }
        public Dictionary<string, AssetBundle> AssetBundles { get; private set; }
        public Dictionary<string, GameObject> Prefabs { get; private set; }

        private AssetBundleManager()
        {
            AssetBundles = new();
            Prefabs = new();
        }

        public static void Initialize()
        {
            Instance = new();
        }

        public Dictionary<string, AssetBundle> LoadAllBundles(string path, bool loadObjects = true)
        {
            try
            {
                Dictionary<string, AssetBundle> result = new();
                AssetBundle bundle;

                if (!Directory.Exists(path))
                    throw new DirectoryNotFoundException();

                using (var plog = new ProcessLogger(Main.Logger))
                {
                    plog.Log($"Loading all AssetBundles from {path} directory");

                    foreach (var file in Directory.GetFiles(path)
                        .Where(f => !f.EndsWith(".manifest")))
                    {
                        if (AssetBundles.ContainsKey(file))
                        {
                            Main.Logger.Error($"AssetBundle {file} has already been loaded.");
                            continue;
                        }

                        plog.Log($"Loading AssetBundle: {file}");

                        if ((bundle = LoadBundle(file, loadObjects)) != null)
                            result.Add(file, bundle);
                    }
                }

                AssetBundles.AddRange(result);

                return result;
            }
            catch (Exception e)
            {
                Main.Logger.Error($"Error Loading AssetBundles from {path} {e.Message}");
                return null;
            }
        }

        public AssetBundle LoadBundle(string fileName, bool loadObjects = true)
        {
            try
            {
                var file = Path.GetFileName(fileName);

                if (AssetBundles.ContainsKey(file))
                    return AssetBundles[file];

                if (!File.Exists(fileName))
                    throw new FileNotFoundException();

                var bundle = AssetBundle.LoadFromFile(fileName);
                AssetBundles.Add(file, bundle);

                if (loadObjects)
                {
                    try
                    {
                        foreach (var asset in bundle.GetAllAssetNames())
                        {
                            if (Prefabs.ContainsKey(asset))
                            {
                                Main.Logger.Error($"Asset {asset} has already been loaded...");
                                continue;
                            }

                            var loaded = bundle.LoadAsset<GameObject>(asset);

                            if (loaded != null)
                            {
                                loaded.SetActive(false);
                                Prefabs.Add(loaded.name, loaded);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }

                return bundle;
            }
            catch (Exception e)
            {
                Main.Logger.Error($"Error Loading AssetBundle: {fileName} {e.Message}");
                return null;
            }
        }

        public void UnloadAllBundles(bool unloadObjects = true)
        {
            try
            {
                foreach (var bundle in AssetBundles)
                {
                    UnloadBundle(bundle.Key, unloadObjects);
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error($"Error Unloading AssetBundles: {e.Message}");
            }
        }

        public void UnloadBundle(string fileName, bool unloadObjects = true)
        {
            try
            {
                var file = Path.GetFileName(fileName);

                if (!AssetBundles.ContainsKey(file))
                    throw new KeyNotFoundException();

                if (unloadObjects)
                {
                    foreach (var obj in AssetBundles[file].GetAllAssetNames())
                    {
                        if (Prefabs.ContainsKey(obj))
                            Prefabs.Remove(obj);
                    }
                }
                AssetBundles[file].Unload(unloadObjects);
            }
            catch (Exception e)
            {
                Main.Logger.Error($"Error Unloading AssetBundle: {fileName} {e.Message}");
            }
        }
    }
}
