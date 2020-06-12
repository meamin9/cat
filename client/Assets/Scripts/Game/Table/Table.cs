using System.Collections.Generic;
using UnityEngine;
using Base;
using System.Collections;
using Newtonsoft.Json;

namespace Game
{
    public static class Table<T> where T : class, new() {
        public static Dictionary<int, T> all;

        public static void FromAsset(UnityEngine.Object asset) {
            all = JsonConvert.DeserializeObject<Dictionary<int, T>>((asset as TextAsset).text);
        }

        public static T Find(int id) {
            if (all.TryGetValue(id, out T value)) {
                return value;
            }
            return null;
        }

    }

    public static class Table
    {
        public static IEnumerator LoadBaseConfig()
        {
            var t = new TimeDebug();
            yield return AssetMgr.Instance.LoadAsync(nameof(SceneTable) + ".json", Table<SceneTable>.FromAsset);
            yield return AssetMgr.Instance.LoadAsync(nameof(SkinTable) + ".json", Table<SkinTable>.FromAsset);
            t.Print("load config Over");
        }
    }

}
