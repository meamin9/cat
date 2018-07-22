using System;
using NSUnityUtil;

public class AppStage {
    public virtual void OnEnter() {}
    public virtual void OnExit() {}
}

public enum EAppStage {
    InitStage = 0, // 初始化app基本配置，uimgr，uiroot
    UpdateStage, // 检测更新
    LoginStage, //
    GameStage,
}

public class AppStageMgr : Singleton<AppStageMgr>
{
    private AppStage _curStage;
    public void Initialize() {
        //TODO: init loading view
    }

    public void EnterStage(AppStage stage) {
        if (_curStage != null) {
            _curStage.OnExit();
        }
        _curStage = stage;
        _curStage.OnEnter();
    }

    //public void EnterStage<T>(T stage) where T: AppStage {
    //    if (_curStage != null) {
    //        _curStage.OnExit();
    //    }
    //    _curStage = stage;
    //    _curStage.OnEnter();
    //}
}
