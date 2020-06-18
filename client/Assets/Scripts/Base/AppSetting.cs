using Newtonsoft.Json;
using System.IO;

namespace Base {
    /*
     * 游戏基本配置，一个json配置
     */
    [System.Serializable]
    public class AppSetting {
        public static AppSetting Instance { get; private set; }

        public static void Load() {
            var path = Path.Combine(AssetMgr.DefaultDir, nameof(AppSetting) + ".json");
            Instance = JsonConvert.DeserializeObject<AppSetting>(File.ReadAllText(path));
#if UNITY_EDITOR
            Instance = Tools.Setting.Instance.appSetting;
#endif
        }
        
        public string PatchUrl = "http://127.0.0.1/";
        public bool LoadInResource = false;


    }
}
