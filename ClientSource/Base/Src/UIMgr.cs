using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Automata.Base
{
    public interface IBaseUI
    {
        int Id { get; }
        string PrefabPath { get; }
        GameObject gameObject { get; set; }
        void OnAttach();
        void OnDetach();
        void OnShow();
        void OnHide();

    }

    public class UIMgr : Singleton<UIMgr>
    {
        Transform _root;

        List<IBaseUI> _uiList = new List<IBaseUI>();
        List<int> _uiLoadingList = new List<int>();
        Dictionary<int, IBaseUI> _uiCache = new Dictionary<int, IBaseUI>();
        Dictionary<int, System.Func<IBaseUI>> _uiCreatorC = new Dictionary<int, System.Func<IBaseUI>>();

        public UIMgr()
        {
            var ui = GameObject.Find("Canvas");
            Log.FatalAsset(ui != null, "not found Canvas");
            _root = ui.transform;
        }

        public static System.Func<IBaseUI> GenUICreator<T>() where T: IBaseUI, new()
        {
            return () => { return new T(); };
        }

        public void RegistUICreator(int id, System.Func<IBaseUI> creator)
        {
            _uiCreatorC.Add(id, creator);
        }

        public void Show(int uikey)
        {
            var ui = Find(uikey);
            if (ui != null)
            {
                ui.gameObject.SetActive(true);
                ui.OnShow();
                return;
            }
            if (_uiCache.TryGetValue(uikey, out ui))
            {
                _uiCache.Remove(uikey);
                ui.gameObject.SetActive(true);
                ui.OnAttach();
                _uiList.Add(ui);
                ui.OnShow();
                return;
            }

            if (_uiLoadingList.Contains(uikey))
            {
                Log.Warnf("UI is Loding...{0}", uikey);
                return;
            }
            System.Func<IBaseUI> creator;
            if (!_uiCreatorC.TryGetValue(uikey, out creator))
            {
                Log.Errorf("Not Found ui creator {0}", uikey);
                return;
            }
            ui = creator();
            _uiLoadingList.Add(uikey);
            AssetMgr.Instance.LoadAsync(ui.PrefabPath, (asset) => {
                if (!_uiLoadingList.Remove(ui.Id))
                {
                    return;
                }
                var go = GameObject.Instantiate(asset as GameObject, _root);
                go.SetActive(true);
                ui.gameObject = go;
                ui.OnAttach();
                _uiList.Add(ui);
            });
        }

        public IBaseUI Find(int uikey)
        {
            var n = _uiList.Count;
            for (var i = 0; i < n; ++i)
            {
                if (_uiList[i].Id == uikey)
                {
                    return _uiList[i];
                }
            }
            return null;
        }

        public void Hide(int uikey)
        {
            var ui = Find(uikey);
            if (ui == null)
            {
                return;
            }
            ui.OnHide();
            ui.gameObject.SetActive(false);
        }

        public void Close(int uikey)
        {
            var ui = Find(uikey);
            if (ui != null)
            {
                if (ui.gameObject.activeSelf)
                {
                    ui.OnHide();
                    ui.gameObject.SetActive(false);
                }
                _uiList.Remove(ui);
                ui.OnDetach();
                GameObject.Destroy(ui.gameObject);
                return;
            }
            _uiLoadingList.Remove(uikey);
        }

        public void CloseAllUI()
        {
            var uiCount = _uiList.Count;
            for(var i = 0; i < uiCount; ++i)
            {
                var ui = _uiList[i];
                if (ui.gameObject.activeSelf)
                {
                    ui.OnHide();
                    ui.gameObject.SetActive(false);
                }
                _uiList.Remove(ui);
                ui.OnDetach();
                GameObject.Destroy(ui.gameObject, 0);
            }
            _uiList.Clear();
            foreach(var it in _uiCache)
            {
                GameObject.Destroy(it.Value.gameObject, 0);
            }
            _uiCache.Clear();
            _uiLoadingList.Clear();
        }

    }
}

