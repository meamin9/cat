using System.Collections;
using System.Collections.Generic;
using AM.Base;
using UnityEngine;

namespace AM.Game {
    public class UIConfig {
        public UIType UIType;
        public string PrefabPath;
        public UILayer Layer;
        public System.Func<IBaseUI> creator;


        public static Dictionary<UIType, UIConfig> All { get; } = new Dictionary<UIType, UIConfig>();

        public static void RegistUI<T>(UIType uiType, string prefabPath, UILayer layer = UILayer.Window)
            where T : IBaseUI, new() {
            All.Add(uiType, new UIConfig() {
                UIType = uiType,
                PrefabPath = prefabPath,
                Layer = layer,
                creator = () => new T()
            });
        }

        public static void LoadUIConfig() {
            RegistUI<LoadingUI>(UIType.Loading, "JoystickUI.prefab");

        }
    }

    public enum UILayer {
        Background,
        Main, // 主界面常驻UI
        Window, // 弹出窗口
        Dialog, // 对话框
        Count
    }

    public enum UIType {
        Begin,
        Loading, //loading
        Joystick,
        End
    }



}
