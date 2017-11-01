using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIShowLayer {
    Moudle, // 游戏模块界面
    Popup, // 弹框
}

public enum UIShowMode {
    HideOther, // 隐藏掉其他UI，自身关闭后恢复其他UI
    CloseOther, // Close其他UI，自身关闭后也不会恢复
    Default, //
}

public class UIDefine {
    public string PrefabPath;
    public bool Single = true; // 只存在一个实例, 不是单例
    public UIShowMode ShowModel = UIShowMode.Default;
    public UIShowLayer ShowLayer = UIShowLayer.Moudle;
    public bool SwallowTouch = true;
    public bool ShadowMask = true;
    public bool HideKill = true;
    public int Id = 0;
}

public interface IUICom {
    UIDefine Define { get; }
    UIDefine Get();
}

public class UIManager: MonoBehaviour {
	static int idSeed = 100;
    public static int GenUIID() {
        return idSeed++;
    }

    // 单栗
    static UIManager _instance;
    public static UIManager I {
        get {
            return _instance;
        }
    }

    // 在游戏游戏初始化时Init
    public static void Init() {
        if (_instance == null) {
            GameObject uim = Instantiate(Resources.Load("UIFrame", typeof(GameObject))) as GameObject;
            _instance = uim.GetComponent<UIManager>();
            if (_instance == null) {
                Debug.LogError("UIFrame not fond UIManager Component");
            }
        }
    }

    public GameObject UIRoot;
    List<UICom> _uiList;
    Dictionary<string, GameObject> _uiCache;


    public GameObject Show<T>(UIDefine define) where T: IUICom {
        GameObject ui;
        var id = define.Id;
        if (define.Single) {

        }
        switch (define.ShowModel) {
            case UIShowMode.CloseOther:
                HideAllUI();
                break;
            case UIShowMode.HideOther:
                break;
        }

        if (_uiShows.TryGetValue (uiname, out ui)) {
            UIBase com = ui.GetComponent<UIBase> ();
            if (com.Define.Singleton) {
                com.Refresh ();
                return ui;
            } else { // 可以同时存在多个的ui,使用这个ui的conf
                define = com.Define;
            }
        } else if (_uiCache.TryGetValue(uiname, out ui)) {
            _uiCache.Remove (uiname);
            _uiShows.Add (uiname);
            ui.SetActive (true);
            return;
        }
        if (define == null) {
            define = UIConfig.Find (uiname);
        }


    }

    public GameObject CreateUI<T>() where T:IUICom {
        var define = T.Define;
        // TODO: 是否需要使用异步加载
        var asset = Resources.Load(define.PrefabPath, typeof(GameObject));
        Debug.Assert(asset != null, "load assets failed" + define.PrefabPath);
        GameObject ui = Instantiate(asset) as GameObject;
        if (ui == null) {
            Debug.LogError("create prafab failed.", define.PrefabPath);
            return;
        }
        switch (define.ShowModel) {
            case UIShowMode.TempHideOther:
                foreach (var u in _uiList) {
                    u:SetActive(false);
                }
                break;
            case UIShowMode.HideOther:
                HideAllUI();
                break;
        }
        _uiList.Add(ui);
        if (define.ShadowMask) {

        }

    }

    void showMode(UIDefine conf) {
        switch (conf.ShowModel) {
        case UIShowMode.CloseOther:

            break;
        }
        if (conf.ShowModel == UIShowMode.CloseOther) {
        }
    }

    // TIP: 在OnEnable和OnDistroy新建ui会产生未定义的行为
    public void HideAllUI() {
        bool hasDestory = false;
        foreach(var ui in _uiShows.Values) {
            UIBase com = ui.GetComponent<UIBase> ();
            if (com.Define.HideKill) {
                ui.Destroy ();
                hasDestory = true;
            } else {
                ui.SetActive (false);
                _uiCache.Add (com.Name, ui);
            }
        }
        _uiShows.Clear ();
        if (hasDestory) {
            Resources.UnloadUnusedAssets ();
        }
    }

    public void HideUI(string uiname) {
        GameObject ui;
        if (_uiShows.TryGetValue (uiname, out ui)) {
            _uiShows.Remove (uiname);
            UIBase com = ui.GetComponent<UIBase> ();
            if (com.Define.HideKill) {
                ui.Destroy ();
                ui = null; // 释放引用
                com = null;
                Resources.UnloadUnusedAssets ();
            } else {
                _uiCache.Add (com.Name, ui);
                ui.SetActive (false);
            }
        }
    }
}
