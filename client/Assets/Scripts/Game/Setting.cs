using Base;
using System.Collections;
using UnityEngine;

namespace Game {
    public static class Setting {
        public static GameSetting Game { get; private set; }

        public static IEnumerator LoadSetting() {
            yield return null;// AssetMgr.Instance.LoadAsync("Config/" + nameof(GameSetting) + ".asset", (asset) => Game = asset as GameSetting);
        }
    }

    [CreateAssetMenu(fileName = nameof(GameSetting), menuName = "ScriptableObjects/" + nameof(GameSetting), order = 2)]
    public class GameSetting : ScriptableObject {
        [Header("Camera Follow")]
        public float FOV = 34;
        public Vector3 RelativePosition = new Vector3(0, 5, -7);
        public Vector3 Rotation = new Vector3(30, 0, 0);
        [Header("服务器ip")]
        public string ServerIp = "127.0.0.1";
        [Header("服务器Port")]
        public int Port = 7000;
    }
}
