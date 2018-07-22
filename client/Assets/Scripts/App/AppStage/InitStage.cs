using UnityEngine;
using NSNetwork;
using NSUnityUtil;

public class InitStage : AppStage {
    // 闪屏

    private int _stayTime = 1;

    public override void OnEnter() {
        Log.Infof("Enter Init Satage");
        //var s = DontDestroyScene.SingleScene;
        AppConfig.Instance.Initialize();
        UIMgr.Instance.Initialize();

        //Asset
        //CNetwork.Instance.StartConnect();
        //UIMgr.Instance.Show<UILoading>();
        Asset.Instance.LoadAsset("Texture/splash.png", (asset) => {

        });
        Asset.Instance.LoadPrefab("UILoading"); // 预加载
    }
}