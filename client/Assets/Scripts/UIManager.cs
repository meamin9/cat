using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UIManager 挂在canvas上，跨场景管理所有ui显示
/// </summary>
public class UIManager: MonoBehaviour {
	// ui id 生成器， 每一个ui实例都有唯一id
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

    public GameObject UIRoot; // UI Root node
	public GameObject Mask; // 全屏遮罩
	Dictionary<int, UICom> _uiShows;
    Dictionary<int, UICom> _uiCache;
	int index = 0;


    public UICom Show<T>(UIDefine define) where T: UICom 
	{
        var id = define.Id;
		UICom com;
		if (_uiShows.TryGetValue(id, out com)) {
			com.gameObject.transform.SetSiblingIndex(index++);
		} else if (_uiCache.TryGetValue(id, out com)) {
			com.gameObject.SetActive(true);
			com.gameObject.transform.SetSiblingIndex(index++);
		} else { // create ui
			GameObject obj = Instantiate(Resources.Load(define.PrefabPath, typeof(GameObject))) as GameObject;
			com = obj.AddComponent<T>();
			_uiShows.Add(id, com);
			obj.SetActive(true);
		}
		return com;
    }

	public void Hide(UIDefine define) {
		var id = define.Id;
		UICom com;
		if (_uiShows.TryGetValue (id, out com)) {
			_uiShows.Remove(id);
			if (define.HideKill) {
				Destroy(com.gameObject);
				com = null;
				Resources.UnloadUnusedAssets();
			} else {
				_uiCache.Add(id, com);
				com.gameObject.SetActive(false);
			}
		}
	}

    // TIP: 在OnEnable和OnDistroy新建ui会产生未定义的行为
    public void HideAllUI() {
        bool hasDestory = false;
        foreach(var com in _uiShows.Values) {
            if (com.Define.HideKill) {
				Destroy(com.gameObject);
                hasDestory = true;
            } else {
				com.gameObject.SetActive(false);
				_uiCache.Add(com.Define.Id, com);
            }
        }
        _uiShows.Clear();
        if (hasDestory) {
            Resources.UnloadUnusedAssets();
        }
    }

}
