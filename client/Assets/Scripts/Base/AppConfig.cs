using UnityEngine;

namespace AM.Base
{
    /*
     * 游戏基本配置，需要最先被加载
     */
    [CreateAssetMenu(fileName = nameof(AppConfig), menuName = "ScriptableObjects/" + nameof(AppConfig), order = 1)]
    public class AppConfig : ScriptableObject
    {
        public static AppConfig Instance { get; private set; }
        public static void Load(Object asset)
        {
            Instance = asset as AppConfig;
        }

        public static bool IsLoaded()
        {
            return Instance != null;
        }

        [Header("入口地址")]
        public string PatchUrl = "http://127.0.0.1/";
        [Header("读取Resource目录资源")]
        public bool LoadInResource = false;

    }

}

