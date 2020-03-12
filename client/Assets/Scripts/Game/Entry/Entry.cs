using System.Collections.Generic;
using UnityEngine;
using AM.Base;
using LitJson;
using System.Collections;

namespace AM.Game
{
    public static class Entry<T> where T : new() {
        public static Dictionary<int, T> All;

        public static void FromAsset(UnityEngine.Object asset) {
            var list = LitJson.JsonMapper.ToObject<T[]>((asset as TextAsset).text);
            All = new Dictionary<int, T>(list.Length);
            foreach (var it in list) {
                All.Add(it.GetHashCode(), it);
            }
        }

    }

    public static class Entry
    {
        public static IEnumerator LoadBaseConfig()
        {
            var t = new TimeDebug();
            yield return AssetMgr.Instance.LoadAsync(nameof(SceneEntry) + ".json", Entry<SceneEntry>.FromAsset);
            yield return AssetMgr.Instance.LoadAsync(nameof(AvatarEntry) + ".json", Entry<AvatarEntry>.FromAsset);
            t.Print("load config Over");
        }
    }

}
