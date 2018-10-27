using System.Collections;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Automata.Adapter;

namespace Automata.Base
{
    public enum AsyncState
    {
        Processing,
        Finish,
    }
    public class AsyncInfo
    {
        public AsyncState State;
        public System.Action<Object> Callback;
        public Object Value;
        public Coroutine Coroutine;

    }
    public class AssetMgr : Singleton<AssetMgr>
    {

        AssetBundleManifest _manifest;
        Dictionary<string, AsyncInfo> _cacheBundles = new Dictionary<string, AsyncInfo>();
        //Dictionary<string, System.Action<AssetBundle>> _bundleHandles = new Dictionary<string, System.Action<AssetBundle>>();

        Dictionary<string, AsyncInfo> _cacheAssets = new Dictionary<string, AsyncInfo>();
        //Dictionary<string, System.Action<Object>> _assetHandles = new Dictionary<string, System.Action<Object>>();

        Dictionary<string, Object> _cacheResAsset = new Dictionary<string, Object>();
        Dictionary<string, System.Action<Object>> _resHandles = new Dictionary<string, System.Action<Object>>();


        #region 资源版本信息
        public Dictionary<string, BundleEntry> _asset2BundleEntryT;
        public Dictionary<string, BundleEntry> _bundleEntryC;
        public const string VERSION_FILE = "Version";
        public const string ASSETS_FILE = "Bundles.json";

        public AppVersion CurrentVersion;
        public AppVersion OriginVersion;

        public AssetMgr()
        {
            LoadAssetsTable();
        }
        public BundleEntry GetBundleEntry(string asset)
        {
            BundleEntry info;
            if (!_asset2BundleEntryT.TryGetValue(asset, out info))
            {
                Log.Errorf("Not Found Bundle for:{0}", asset);
            }
            return info;
        }

        public static string PatchDir
        {
            get { return Path.Combine(Application.persistentDataPath, "Patch"); }
        }

        public static string DefaultDir
        {
            get { return Application.streamingAssetsPath; }
        }

        public void Clear()
        {
            _manifest = null;
            //foreach(var it in _cacheBundles)
            //{
            //    it.Value.Unload(false);
            //}
            _cacheBundles.Clear();
            //_bundleHandles.Clear();

            _cacheAssets.Clear();
            //_assetHandles.Clear();

            _cacheResAsset.Clear();
            _resHandles.Clear();

            _asset2BundleEntryT.Clear();
            _bundleEntryC.Clear();

            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();
        }


        public void LoadAssetsTable()
        {
            // 1. read current version
            var verPath = Path.Combine(DefaultDir, VERSION_FILE);
            OriginVersion = AppVersion.Parse(File.ReadAllText(verPath));
            verPath = Path.Combine(PatchDir, VERSION_FILE);
            if (File.Exists(verPath))
            {
                CurrentVersion = AppVersion.Parse(File.ReadAllText(verPath));
            }
            else
            {
                CurrentVersion = OriginVersion;
            }

            // 2. 所有资源信息
            var defaultPath = Path.Combine(DefaultDir, ASSETS_FILE);
            var defaultAssets = JsonUtility.FromJson<Dictionary<string, BundleConf>>(File.ReadAllText(defaultPath));

            Dictionary<string, BundleConf> patchBundle = null;
            string patchPath = Path.Combine(PatchDir, ASSETS_FILE + CurrentVersion.ToString());
            if (!File.Exists(patchPath))
            {
                patchBundle = defaultAssets;
            }
            else
            {
                patchBundle = JsonUtility.FromJson<Dictionary<string, BundleConf>>(File.ReadAllText(patchPath));
            }
            BundleConf defaultConf;
            foreach (var it in patchBundle)
            {
                var name = it.Key;
                var conf = it.Value;
                bool isPatch = true;
                if (defaultAssets.TryGetValue(name, out defaultConf) && defaultConf.Version == conf.Version)
                {
                    isPatch = false;
                }
                var entry = new BundleEntry(name, conf, isPatch);
                _bundleEntryC[name] = entry;
                var assetsCount = conf.Assets.Length;
                for (int i = 0; i < assetsCount; ++i)
                {
                    _asset2BundleEntryT[conf.Assets[i]] = entry;
                }
            }
            LoadAssetAsync("AssetBundleManifest", (asset) =>
            {
                _manifest = asset as AssetBundleManifest;
            });
        }
        #endregion

        private List<AsyncOperation> _sceneAsyncList = new List<AsyncOperation>();
        public void LoadSceneAsync(string sceneName, System.Action callback)
        {
            BundleEntry entry;
            if (!_asset2BundleEntryT.TryGetValue(sceneName, out entry))
            {
                Log.Errorf("Not Found Bundle for:{0}", sceneName);
                callback?.Invoke();
                return;
            }
            LoadBundleAsync(entry, (bundle) =>
            {
                MonoProxy.Instance.StartCoroutine(_loadSceneCoroutine(sceneName, callback));
            });
        }

        private IEnumerator _loadSceneCoroutine(string sceneName, System.Action callback)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            _sceneAsyncList.Add(operation);
            yield return operation;
            _sceneAsyncList.Remove(operation);
            callback?.Invoke();
        }
        public float SceneLoadProgress()
        {
            if (_sceneAsyncList.Count == 0)
            {
                return 1.0f;
            }
            return _sceneAsyncList[_sceneAsyncList.Count - 1].progress;
        }

        public int LoadingCount()
        {
            return _cacheAssets.Count;
        }
        public Coroutine LoadAssetAsync(string assetName, System.Action<Object> callback)
        {
            AsyncInfo info;
            if (_cacheAssets.TryGetValue(assetName, out info))
            {
                if (info.State == AsyncState.Finish)
                {
                    callback?.Invoke(info.Value);
                    return null;
                }
                else
                {
                    if (info.Callback == null)
                    {
                        info.Callback = callback;
                    }
                    else
                    {
                        info.Callback += callback;
                    }
                    return info.Coroutine;
                }
            }
            BundleEntry entry;
            if (!_asset2BundleEntryT.TryGetValue(assetName, out entry))
            {
                Log.Errorf("Not Found Bundle for:{0}", assetName);
                return null;
            }
            _cacheAssets.Add(assetName, info);
            info.State = AsyncState.Processing;
            info.Callback = callback;
            info.Coroutine = LoadBundleAsync(entry, (bundle) =>
            {
                MonoProxy.Instance.StartCoroutine(_loadAssetCoroutine(bundle as AssetBundle, assetName));
            });
            return info.Coroutine;
        }

        private IEnumerator _loadAssetCoroutine(AssetBundle bundle, string assetName)
        {
            var req = bundle.LoadAssetAsync(assetName);
            yield return req;
            var asset = req.asset;
            if (asset == null)
            {
                Log.Errorf("Failed to load asset:{0}", assetName);
            }
            AsyncInfo info;
            if (_cacheAssets.TryGetValue(assetName, out info))
            {
                info.Coroutine = null;
                info.Value = asset;
                info.State = AsyncState.Finish;
                var callback = info.Callback;
                info.Callback = null;
                callback?.Invoke(asset);
            }
        }

        public Coroutine LoadBundleAsync(string bundleName, System.Action<Object> handle)
        {
            BundleEntry entry;
            if (!_bundleEntryC.TryGetValue(bundleName, out entry))
            {
                Log.Error("Not Founf AssetBundles");
                return null;
            }
            return LoadBundleAsync(entry, handle);
        }

        public Coroutine LoadBundleAsync(BundleEntry bundleEntry, System.Action<Object> callback)
        {
            string name = bundleEntry.Name;

            AsyncInfo info;
            if (_cacheBundles.TryGetValue(name, out info))
            {
                if (info.State == AsyncState.Finish)
                {
                    callback?.Invoke(info.Value);
                    return null;
                }
                else
                {
                    if (info.Callback == null)
                    {
                        info.Callback = callback;
                    }
                    else
                    {
                        info.Callback += callback;
                    }
                    return info.Coroutine;
                }
            }

            _cacheAssets.Add(name, info);
            info.State = AsyncState.Processing;
            info.Callback = callback;
            info.Coroutine = MonoProxy.Instance.StartCoroutine(_loadBundleCoroutine(bundleEntry));
            return info.Coroutine;
        }

        private IEnumerator _loadBundleCoroutine(BundleEntry entry)
        {
            var name = entry.Name;
            while (_manifest == null)
            {
                yield return null;
            }
            if (_manifest != null)
            {
                string[] dependencies = _manifest.GetAllDependencies(name);
                var c = dependencies.Length;
                for (var i = 0; i < c; ++i)
                {
                    yield return LoadBundleAsync(dependencies[i], null);
                }
            }
            var req = AssetBundle.LoadFromFileAsync(entry.Url);
            yield return req;
            var bundle = req.assetBundle;
            if (bundle == null)
            {
                Log.Errorf("Failed to load asset bundle:{0}", entry);
            }
            AsyncInfo info;
            if (_cacheAssets.TryGetValue(name, out info))
            {
                info.Coroutine = null;
                info.Value = bundle;
                info.State = AsyncState.Finish;
                var callback = info.Callback;
                info.Callback = null;
                callback?.Invoke(bundle);
            }
        }

        public void LoadResource(string assetName, System.Action<Object> handle)
        {
            var path = assetName; //Path.Combine(Application.streamingAssetsPath, assetName);
            if (!_TryGetAsset(_cacheResAsset, _resHandles, path, handle))
            {
                MonoProxy.Instance.StartCoroutine(_loadResCoroutine(path));
            }
        }
        private IEnumerator _loadResCoroutine(string assetPath)
        {
            var req = Resources.LoadAsync(assetPath);
            yield return req;
            var asset = req.asset;
            _LoadFinished(_cacheResAsset, _resHandles, assetPath, asset);
        }
        private bool _TryGetAsset<T>(Dictionary<string, T> cache, Dictionary<string, System.Action<T>> handles,
                             string key, System.Action<T> handle) where T : class
        {
            T obj;
            if (cache.TryGetValue(key, out obj))
            {
                if (handle != null)
                {
                    if (obj != null)
                    {
                        handle(obj);
                    }
                    else
                    {
                        handles[key] += handle;
                    }
                }
                return true;
            }
            cache[key] = null;
            if (handle != null)
            {
                handles[key] = handle;
            }
            return false;
        }

        private void _LoadFinished<T>(Dictionary<string, T> cache, Dictionary<string, System.Action<T>> handles,
                              string key, T asset) where T : class
        {
            System.Action<T> handle;
            if (handles.TryGetValue(key, out handle))
            {
                handles.Remove(key);
                cache[key] = asset; // 是否缓存
                handle(asset);
            }
        }

        public Coroutine LoadAsync(string path, System.Action<Object> handle)
        {
            //if (AppConfig.Instance.IsResMode)
            //{
            //    LoadResource(path, handle);
            //    return;
            //}
            return LoadAssetAsync(path, handle);
        }

    }

}

