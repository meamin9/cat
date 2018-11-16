using System.Collections;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Automata.Adapter;
using LitJson;

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
        public System.Action<AsyncInfo> Callback;
        public Object Asset;
        public Coroutine Coroutine;

    }
    public class AssetMgr : Singleton<AssetMgr>
    {

        AssetBundleManifest _manifest;
        Dictionary<string, AsyncInfo> _cacheBundles = new Dictionary<string, AsyncInfo>();

        Dictionary<string, AsyncInfo> _cacheAssets = new Dictionary<string, AsyncInfo>();

        Dictionary<string, AsyncInfo> _cacheRes = new Dictionary<string, AsyncInfo>();


        #region 资源版本信息
        public Dictionary<string, BundleEntry> _asset2BundleEntryT = new Dictionary<string, BundleEntry>();
        public Dictionary<string, BundleEntry> _bundleEntryC = new Dictionary<string, BundleEntry>();
        public const string VERSION_FILE = "VERSION";
        public const string ASSETS_FILE = "VERSION.json";

        public AppVersion CurrentVersion;
        public AppVersion OriginVersion;

        //public AssetMgr()
        //{
        //    LoadAssetsTable();
        //}
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

        private static string _defaultDir;
        public static string DefaultDir
        {
            get
            {
                if (_defaultDir == null)
                {
#if UNITY_IPHONE || UNITY_EDITOR_OSX
                    _defaultDir = Path.Combine(Application.streamingAssetsPath, "IOS", "AssetBundles");
#else
                    _defaultDir = Path.Combine(Application.streamingAssetsPath, "Android", "AssetBundles");
#endif
                }
                return _defaultDir;
            }
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

            _cacheRes.Clear();
            //_resHandles.Clear();

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
            var defaultAssets = LitJson.JsonMapper.ToObject<Dictionary<string, BundleConf>>(File.ReadAllText(defaultPath));

            Dictionary<string, BundleConf> patchBundle = null;
            string patchPath = Path.Combine(PatchDir, ASSETS_FILE + CurrentVersion.ToString());
            if (!File.Exists(patchPath))
            {
                patchBundle = defaultAssets;
            }
            else
            {
                patchBundle = LitJson.JsonMapper.ToObject<Dictionary<string, BundleConf>>(File.ReadAllText(patchPath));
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
                _bundleEntryC.Add(name, entry);
                var assetsCount = conf.Assets != null ? conf.Assets.Length : 0; ;
                for (int i = 0; i < assetsCount; ++i)
                {
                    _asset2BundleEntryT[conf.Assets[i]] = entry;
                }
            }
//            LoadAssetAsync("AssetBundleManifest", (asset) =>
//            {
//                _manifest = asset as AssetBundleManifest;
//            });
//#if UNITY_EDITOR
//            AppConfig.Load(Resources.Load("AppConfig"));
//#else
//            LoadAssetAsync("AppConfig.asset", (asset) => {
//                AppConfig.Load(asset);
//            });
//#endif
        }
        public Coroutine LoadAssetManifest()
        {
            return LoadAssetAsync("AssetBundleManifest", (req) =>
            {
                _manifest = req.Asset as AssetBundleManifest;
            });
        }
#endregion

        //private List<AsyncOperation> _sceneAsyncList = new List<AsyncOperation>();
        //public void LoadSceneAsync(string sceneName, System.Action callback)
        //{
        //    BundleEntry entry;
        //    if (!_asset2BundleEntryT.TryGetValue(sceneName, out entry))
        //    {
        //        Log.Errorf("Not Found Bundle for:{0}", sceneName);
        //        callback?.Invoke();
        //        return;
        //    }
        //    LoadBundleAsync(entry, (bundle) =>
        //    {
        //        MonoProxy.Instance.StartCoroutine(_loadSceneCoroutine(sceneName, callback));
        //    });
        //}

        //private IEnumerator _loadSceneCoroutine(string sceneName, System.Action callback)
        //{
        //    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        //    _sceneAsyncList.Add(operation);
        //    yield return operation;
        //    _sceneAsyncList.Remove(operation);
        //    callback?.Invoke();
        //}
        //public float SceneLoadProgress()
        //{
        //    if (_sceneAsyncList.Count == 0)
        //    {
        //        return 1.0f;
        //    }
        //    return _sceneAsyncList[_sceneAsyncList.Count - 1].progress;
        //}

        //public int LoadingCount()
        //{
        //    return _cacheAssets.Count;
        //}
        public Coroutine LoadAssetAsync(string assetName, System.Action<AsyncInfo> callback)
        {
            AsyncInfo info;
            if (_cacheAssets.TryGetValue(assetName, out info))
            {
                if (info.State == AsyncState.Finish)
                {
                    callback?.Invoke(info);
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
            info = new AsyncInfo();
            _cacheAssets.Add(assetName, info);
            info.State = AsyncState.Processing;
            info.Callback = callback;
            info.Coroutine = LoadBundleAsync(entry, (req) =>
            {
                req.Coroutine = MonoProxy.Instance.StartCoroutine(_loadAssetCoroutine(req.Asset as AssetBundle, assetName));
            });
            return info.Coroutine;
        }

        private IEnumerator _loadAssetCoroutine(AssetBundle bundle, string assetName)
        {
            //Log.Infof("bundle:{0}>>", bundle);
            //var names = bundle.GetAllAssetNames();
            //foreach(var n in names)
            //{
            //    Log.Info(n);
            //}
            //if(assetName == "Views/UILoading.prefab")
            //{
            //    assetName = "UILoading.prefab";
            //}
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
                info.Asset = asset;
                info.State = AsyncState.Finish;
                var callback = info.Callback;
                info.Callback = null;
                callback?.Invoke(info);
                if (info.Coroutine != null)
                {
                    yield return info.Coroutine; // 回调里可以设置coroutine的值，在这里会继续等待完成
                    info.Coroutine = null;
                }
            }
        }

        public Coroutine LoadBundleAsync(string bundleName, System.Action<AsyncInfo> handle)
        {
            BundleEntry entry;
            if (!_bundleEntryC.TryGetValue(bundleName, out entry))
            {
                Debug.LogErrorFormat("Not Found AssetBundles:{0}", bundleName);
                return null;
            }
            return LoadBundleAsync(entry, handle);
        }

        public Coroutine LoadBundleAsync(BundleEntry bundleEntry, System.Action<AsyncInfo> callback)
        {

            string name = bundleEntry.Name;
#if UNITY_EDITOR
            Debug.LogFormat("Begin Load Bundle:{0}", name);
            var begin = Time.time;
            callback += (obj) => {
                Debug.LogFormat("End Load Bundle:{0}, Cost Time:{1}", name, Time.time - begin);
            };
#endif
            AsyncInfo info;
            if (_cacheBundles.TryGetValue(name, out info))
            {
                if (info.State == AsyncState.Finish)
                {
                    callback?.Invoke(info);
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
            info = new AsyncInfo();
            _cacheBundles.Add(name, info);
            info.State = AsyncState.Processing;
            info.Callback = callback;
            info.Coroutine = MonoProxy.Instance.StartCoroutine(_loadBundleCoroutine(bundleEntry));
            return info.Coroutine;
        }

        private IEnumerator _loadBundleCoroutine(BundleEntry entry)
        {
            var name = entry.Name;
            while (_manifest == null && name != "AssetBundles")
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
            if (_cacheBundles.TryGetValue(name, out info))
            {
                info.Coroutine = null;
                info.Asset = bundle;
                info.State = AsyncState.Finish;
                var callback = info.Callback;
                info.Callback = null;
                callback?.Invoke(info);
                if (info.Coroutine != null)
                {
                    yield return info.Coroutine; // 回调里可以设置coroutine的值，在这里会继续等待完成
                    info.Coroutine = null;
                }
            }
        }

        public Coroutine LoadResourceAsync(string assetName, System.Action<AsyncInfo> callback)
        {
            //var assetName = assetName;
            AsyncInfo info;
            if (_cacheRes.TryGetValue(assetName, out info))
            {
                if (info.State == AsyncState.Finish)
                {
                    callback?.Invoke(info);
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
            info = new AsyncInfo();
            _cacheAssets.Add(assetName, info);
            info.State = AsyncState.Processing;
            info.Callback = callback;
            info.Coroutine = MonoProxy.Instance.StartCoroutine(_loadResCoroutine(assetName));
            return info.Coroutine;
        }
        private IEnumerator _loadResCoroutine(string assetName)
        {
            var req = Resources.LoadAsync(assetName);
            yield return req;
            var asset = req.asset;
            AsyncInfo info;
            if (_cacheAssets.TryGetValue(assetName, out info))
            {
                info.Coroutine = null;
                info.Asset = asset;
                info.State = AsyncState.Finish;
                var callback = info.Callback;
                info.Callback = null;
                callback?.Invoke(info);
                if (info.Coroutine != null)
                {
                    yield return info.Coroutine;
                    info.Coroutine = null;
                }
            }
        }

        private Dictionary<string, string> _resFullPaths;
        private Dictionary<string, string> ResFullPaths
        {
            get
            {
                if (_resFullPaths == null)
                {
                    _resFullPaths = new Dictionary<string, string>();
                    var resDir = "Assets/Resources/";
                    var baseLen = resDir.Length;
                    var dir = resDir;
                    var files = Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var ext = Path.GetExtension(file);
                        if (ext.Equals(".meta"))
                        {
                            continue;
                        }
                        var name = Path.GetFileName(file);
                        var v = file.Substring(baseLen, file.Length - baseLen - ext.Length);
                        _resFullPaths.Add(name, v);
                    }
                }
                return _resFullPaths;
            }
        }
        /// <summary>
        /// 加载asset资源统一接口
        /// </summary>
        public Coroutine LoadAsync(string path, System.Action<AsyncInfo> handle)
        {
#if UNITY_EDITOR
            Debug.LogFormat("Begin Load Asset:{0}", path);
            var begin = Time.time;
            handle += (obj) => {
                Debug.LogFormat("End Load Asset:{0}, Cost Time:{1}", path, Time.time - begin);
            };
#endif
            if (BaseConfig.Instance.LoadInResource)
            {
                path = ResFullPaths[path];
                return LoadResourceAsync(path, handle);
            }
            return LoadAssetAsync(path, handle);
        }

    }

}

