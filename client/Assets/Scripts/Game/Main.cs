using AM.Base;
using System.Collections;

namespace AM.Game {
    public class Main {
        public static void Start() {
            MonoProxy.Instance.StartCoroutine(Initialize());
        }

        private static IEnumerator Initialize() {
            Log.Info("Game Start");
            yield return AssetMgr.InitAsync();
            Log.Info("AssetMgr Init ...ok");
            // 加载所有setting
            yield return MonoProxy.Instance.StartCoroutine(Setting.LoadSetting());
            Log.Info("Load Setting ...ok");
            RandomExt.Init();
            TimeExt.Init();
            UIManager.Init();
            // 连接网络
            //Network.Connect();
            // 加载配表1
            yield return MonoProxy.Instance.StartCoroutine(Entry.LoadBaseConfig());
            Log.Info("Load Table ...ok");

            //Initialize();
            SceneManager.SwitchScene(100);

        }
    }
}

