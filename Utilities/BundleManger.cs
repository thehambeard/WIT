using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static WIT.Utilities.SettingsWrapper;
using static WIT.Main;
using System.IO;
using System.Reflection;

namespace WIT.Utilities
{
    public static class BundleManger
    {
        private static Dictionary<string, GameObject> m_objects = new Dictionary<string, GameObject>();
        private static Dictionary<string, Sprite> m_sprites = new Dictionary<string, Sprite>();

        public static void RemoveBundle(string loadAss, bool unloadAll = false)
        {
            
            AssetBundle bundle;
            if (bundle = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(x => x.name == loadAss))
                bundle.Unload(unloadAll);
            if (unloadAll)
            {
                m_objects.Clear();
                m_sprites.Clear();
            }
        }

        public static void AddBundle(string loadAss)
        {
            
            try
            {
                AssetBundle bundle;
                GameObject prefab;
                Sprite sprite;
                
                RemoveBundle(loadAss, true);

                bundle = AssetBundle.LoadFromFile(ModPath + loadAss);
                if (!bundle) throw new Exception($"Failed to load AssetBundle! {ModPath + loadAss}");
                foreach (string b in bundle.GetAllAssetNames())
                {
                    if (b.EndsWith(".prefab"))
                    {
                        Mod.Debug($"Loading prefab: {b}");
                        if (!m_objects.ContainsKey(Path.GetFileNameWithoutExtension(b)))
                        {
                            if ((prefab = bundle.LoadAsset<GameObject>(b)) != null)
                            {
                                prefab.SetActive(false);
                                m_objects.Add(prefab.name, prefab);
                            }
                            else
                                Mod.Error($"Failed to load the prefab: {b}");
                        }
                        else
                            Mod.Error($"Asset {b} already loaded.");
                    }
                    if (b.EndsWith(".png"))
                    {
                        if (!m_sprites.ContainsKey(Path.GetFileNameWithoutExtension(b)))
                        {
                            if ((sprite = bundle.LoadAsset<Sprite>(b)) != null)
                            {
                                m_sprites.Add(sprite.name, sprite);
                            }
                            else
                                Mod.Error($"Failed to load the prefab: {b}");
                        }
                        else
                            Mod.Error($"Asset {b} already loaded.");
                    }
                }

                RemoveBundle(loadAss);
            }
            catch (Exception ex)
            {
                Main.Mod.Error(ex.Message + ex.StackTrace);
            }
        }
        
        public static bool IsLoaded(string asset)
        {
            return m_objects.ContainsKey(asset);
        }

        public static Dictionary<string, GameObject> LoadedPrefabs { get { return m_objects; } }
        public static Dictionary<string, Sprite> LoadedSprites { get { return m_sprites; } }
    }
}
