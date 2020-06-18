using System.Collections.Generic;
using UnityEngine;
using Base;
using System.Collections;
using Newtonsoft.Json;

namespace Game
{
    public static class Table<T> where T : class, new() {
        private static Dictionary<int, T> dict;
        private static T[] arr;

        public static void FromAsset(UnityEngine.Object asset) {
            var text = (asset as TextAsset).text;
            if (text[0] == '[') {
                arr = JsonConvert.DeserializeObject<T[]>(text);
            }
            else {
                dict = JsonConvert.DeserializeObject<Dictionary<int, T>>(text);
            }
        }

        public static T Find(int id) {
            if (arr != null && 0 <= id &&id < arr.Length) {
                return arr[id];
            }
            else if (dict != null && dict.TryGetValue(id, out T value)) {
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
            yield return AssetMgr.Instance.LoadAsync(nameof(tScene) + ".json", Table<tScene>.FromAsset);
            yield return AssetMgr.Instance.LoadAsync(nameof(tRole) + ".json", Table<tRole>.FromAsset);
            yield return AssetMgr.Instance.LoadAsync(nameof(tSkin) + ".json", Table<tSkin>.FromAsset);
            yield return AssetMgr.Instance.LoadAsync(nameof(tAct) + ".json", Table<tAct>.FromAsset);
            t.Print("load config Over");
        }
    }

}
