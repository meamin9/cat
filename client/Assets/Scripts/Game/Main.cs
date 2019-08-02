using AM.Base;
using System.Collections;

namespace AM.Game
{
    public class Main
    {
        public static void Start()
        {
            MonoProxy.Instance.StartCoroutine(Initialize());
        }

        private static IEnumerator Initialize()
        {
            yield return AssetMgr.InitAsync();

            Log.Info("Game Start");
            // base moudle
            RandomExt.Init();
            TimeExt.Init();

            UIManager.Init();
            //Network.Connect();
            // 加载配表1
            yield return MonoProxy.Instance.StartCoroutine(Conf.LoadBaseConfig());

            //Initialize();
            
        }
    }
}

