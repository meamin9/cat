using UnityEngine;

namespace Automata.Base
{
    [CreateAssetMenu(fileName = nameof(BaseConfig), menuName = "ScriptableObjects/" + nameof(BaseConfig), order = 1)]
    public class BaseConfig : ScriptableObject
    {
        public static BaseConfig Instance { get; private set; }
        public static void Load(Object asset)
        {
            Instance = asset as BaseConfig;
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

