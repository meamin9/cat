using System.Collections.Generic;
using UnityEngine;
using AssetMgr = AM.Base.AssetMgr;

namespace AM.Game
{
    public static class UIManager
    {
        struct LoadingInfo
        {
            public UIType UIType;
            public System.Action<IBaseUI> Callback;
        }

        static Transform _root;
        static List<IBaseUI> _uiList = new List<IBaseUI>();
        static List<LoadingInfo> _uiLoadings = new List<LoadingInfo>(); //正在加载的界面
        static Dictionary<UIType, IBaseUI> _uiCache = new Dictionary<UIType, IBaseUI>();
        
        public static void Init()
        {
            var ui = GameObject.Find("Canvas");
            Log.ErrorIf(!ui, "not found Canvas");
            _root = ui.transform;
            InitLayer();
            UIConfig.LoadUIConfig();
        }
        public static Transform Layer(UILayer layer)
        {
            return _root.GetChild((int)layer);
        }

        private static void InitLayer()
        {
            var count = (int)UILayer.Count;
            for (var i = 0; i < count; ++i) {
                var layer = new GameObject("Layer_" + i);
                layer.transform.parent = _root;
            }
        }

        public static void Show(UIType uitype, System.Action<IBaseUI> callback=null)
        {
            var ui = Find(uitype);
            if (ui != null) {
                ui.gameObject.SetActive(true);
                ui.OnShow();
                callback?.Invoke(ui);
                return;
            }
            if (_uiCache.TryGetValue(uitype, out ui)) {
                _uiCache.Remove(uitype);
                _uiList.Add(ui);
                ui.gameObject.SetActive(true);
                ui.gameObject.transform.SetAsLastSibling();
                ui.OnShow();
                callback?.Invoke(ui);
                return;
            }
            for (var i = 0; i < _uiLoadings.Count; ++i) {
                if (_uiLoadings[i].UIType == uitype) {
                    _uiLoadings[i] = new LoadingInfo {
                        UIType = uitype,
                        Callback = callback
                    };
                    Log.Warn("UI is Loding...{0}", uitype);
                    return;
                }
            }
            var info = UIConfig.All[uitype];
            _uiLoadings.Add(new LoadingInfo() {UIType=uitype, Callback=callback});
            AssetMgr.Instance.LoadAsync(info.PrefabPath, (req) => {
                for (var i = 0; i < _uiLoadings.Count; ++i) {
                    if (_uiLoadings[i].UIType == uitype) {
                        ui = info.creator();
                        ui.gameObject = GameObject.Instantiate(req.Asset as GameObject, Layer(info.Layer));
                        ui.UIType = info.UIType;
                        _uiList.Add(ui);
                        ui.OnAttach();
                        ui.OnShow();
                        _uiLoadings[i].Callback?.Invoke(ui);
                        _uiLoadings.RemoveAt(i);
                        return;
                    }
                }
                Log.Warn($"ui{uitype}已经加载，但界面已经关了");
            });
        }

        public static IBaseUI Find(UIType type)
        {
            var n = _uiList.Count;
            for (var i = 0; i < n; ++i) {
                if (_uiList[i].UIType == type) {
                    return _uiList[i];
                }
            }
            return null;
        }

        public static void Close(UIType uitype)
        {
            var n = _uiLoadings.Count;
            for (var i = n - 1; i >=0; --i) {
                if (_uiLoadings[i].UIType == uitype) {
                    _uiLoadings.RemoveAt(i);
                    return;
                }
            }
            n = _uiList.Count;
            for (var i = n-1; i >= 0; --i) {
                if (_uiList[i].UIType == uitype) {
                    DestroyUI(_uiList[i]);
                    _uiList.RemoveAt(i);
                    return;
                }
            }
        }

        private static void DestroyUI(IBaseUI ui)
        {
            ui.OnHide();
            if (ui.gameObject.transform.parent == Layer(UILayer.Main)) {
                ui.gameObject.SetActive(false);
                _uiCache[ui.UIType] = ui;
            }
            else {
                ui.OnDetach();
                GameObject.Destroy(ui.gameObject);
            }
        }
    }
}
