using UnityEngine;
using AM.Base;

namespace AM.Game {
    [CreateAssetMenu(fileName = nameof(GameSetting), menuName = "ScriptableObjects/" + nameof(GameSetting), order = 2)]
    public class GameSetting : ScriptableObject
    {
        public static GameSetting Instance { get; private set; }
        public static void Load(AsyncInfo asset) {
            Instance = asset as GameSetting;
        }
        [Header("服务器ip")]
        public string ServerIp = "127.0.0.1";
        [Header("服务器Port")]
        public int Port = 7000;
    }

}

