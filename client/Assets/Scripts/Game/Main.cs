﻿using Base;
using System.Collections;

namespace Game {
    public class Main {
        public static void Start() {
            MonoProxy.Instance.StartCoroutine(Initialize());
        }

        private static IEnumerator Initialize() {
            Log.Info("Game Start");
            AppSetting.Load();
            yield return AssetMgr.Instance.InitAsync();
            Log.Info("AssetMgr Init ...ok");
            // 加载所有setting
            yield return MonoProxy.Instance.StartCoroutine(Setting.LoadSetting());
            Log.Info("Load Setting ...ok");
            RandomExt.Init();
            GameTime.Init();
            UIManager.Init();

            // 连接网络
            //Network.Connect();
            // 加载配表1
            yield return MonoProxy.Instance.StartCoroutine(Table.LoadBaseConfig());
            Log.Info("Load Table ...ok");

            SceneManager.LoadGloableSceneNode();
            //Initialize();
            SceneManager.SwitchScene(0);

            SceneManager.OnSceneLoaded += () => { EntityManager.Instance.CreateRole(0); };

        }
    }
}

