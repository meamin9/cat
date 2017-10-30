using UnityEngine;
using System.Collections.Generic;

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
    public bool Singleton = true; // 只存在一个实例
    public UIShowMode ShowModel = ShowModel.Default;
    public UIShowLayer ShowLayer = UIShowLayer.Moudle;
    public bool SwallowTouch = true;
    public bool ShadowMask = true;
    public bool HideKill = true;
}

public interface IUICom {
	UIDefine Define { get; }
}

public class UIManager {
    static readonly UIManager _instance = new UIManager();
    public static UIManager I {
        get {
            return _instance;
        }
    }

    Dictionary<string, GameObject> _uiCache;
    Dictionary<string, GameObject> _uiShows;


	public GameObject Show(string uiname) {
		GameObject ui;
		UIDefine define;
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
		var asset = Resources.LoadAsync
		
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
