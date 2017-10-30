using UnityEngine;

namespace Game {
    public enum UIShowLayer {
        Moudle, // 游戏模块界面
        Popup, // 弹框
    }
    public enum UIShowMode {
        HideOther, // 隐藏掉其他UI，自身关闭后恢复其他UI
        CloseOther, // Close其他UI，自身关闭后也不会恢复
        Default, //
    }
    public calss UIDefine {
        public string PrefabName;
        public bool Singleton = true; // 只存在一个实例
        public UIShowMode ShowModel = ShowModel.Default;
        public UIShowLayer ShowLayer = UIShowLayer.Moudle;
        public bool SwallowTouch = true;
        public bool ShadowMask = true;
        public bool HideKill = true;
    }

    public class UIManager {
        static readonly UIManager _instance = new UIManager();
        public static I {
            get {
                return _instance;
            }
        }

        Dictionary<string, GameObject> _uiCache;
        Dictionary<string, GameObject> _uiShows;

        public void Show(string prefabName){}
        public void Hide(){}
        public void Create(string prefabName){}
        public void Destroy() {}
    }
}
