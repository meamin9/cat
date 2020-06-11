using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace Base {
    /*
     * 游戏基本配置，对应一个json文件
     */
    [CreateAssetMenu(fileName = nameof(AppSetting), menuName = "ScriptableObjects/" + nameof(AppSetting), order = 1)]
    public class AppSetting : ScriptableObject {

        private static AppSetting _instance;
        public static AppSetting Instance {
            get {
                if (_instance == null) {
                    Load();
                }
                return _instance;
            }
        }
        public static void Load() {
            var path = Path.Combine(AssetMgr.DefaultDir, nameof(AppSetting) + ".json");
#if UNITY_EDITOR
            _instance = Resources.Load<AppSetting>("Config/" + nameof(AppSetting));
            var json = JsonConvert.SerializeObject(Instance);
            File.WriteAllText(path, json);
#endif
            _instance = JsonConvert.DeserializeObject<AppSetting>(File.ReadAllText(path));
        }

#if UNITY_EDITOR
        public void OnValidate() {
            var path = Path.Combine(AssetMgr.DefaultDir, nameof(AppSetting) + ".json");
            _instance = Resources.Load<AppSetting>("Config/" + nameof(AppSetting));
            var json = JsonConvert.SerializeObject(Instance);
            File.WriteAllText(path, json);
        }
#endif

        [Header("入口地址")]
        public string PatchUrl = "http://127.0.0.1/";
        [Header("读取Resource资源")]
        public bool LoadInResource = false;


    }
}
