﻿using System.Collections;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace AM.Base
{
    public class AssetMgr
    {
        private enum AsyncState {
            Processing,
            Finish,
        }

        private struct AsyncInfo {
            public AsyncState State;
            public System.Action<Object> Callback;
            public Object Asset;
            public Coroutine Coroutine;

        }

        public static readonly AssetMgr Instance = new AssetMgr();

        AssetBundleManifest _manifest;
        Dictionary<string, AsyncInfo> _cacheBundles = new Dictionary<string, AsyncInfo>();

        Dictionary<string, AsyncInfo> _cacheAssets = new Dictionary<string, AsyncInfo>();

        Dictionary<string, AsyncInfo> _cacheRes = new Dictionary<string, AsyncInfo>();


        #region 资源版本信息
        public Dictionary<string, BundleInfo> _asset2BundleInfoT = new Dictionary<string, BundleInfo>();
        public Dictionary<string, BundleInfo> _bundleEntryC = new Dictionary<string, BundleInfo>();
        public const string VERSION_FILE = "VERSION";
        public const string ASSETS_FILE = "VERSION.json";

        public AppVersion CurrentVersion;
        public AppVersion OriginVersion;

        public static Coroutine InitAsync()
        {
            AssetMgr.Instance.LoadAssetsTable();
            return MonoProxy.Instance.StartCoroutine(LoadAppConfig());
        }

        private static IEnumerator LoadAppConfig()
        {
            yield return AssetMgr.Instance.LoadAssetManifest();
#if UNITY_EDITOR
            var asset = Resources.Load("Config/" + nameof(AppSetting));
            AppSetting.Load(asset);
#endif
            if (!AppSetting.IsLoaded() || !AppSetting.Instance.LoadInResource) {
                yield return AssetMgr.Instance.LoadAssetAsync(nameof(AppSetting) + ".asset", (ret) => {
                    AppSetting.Load(ret);
                });
            }
        }

        public BundleInfo GetBundleInfo(string asset)
        {
            BundleInfo info;
            if (!_asset2BundleInfoT.TryGetValue(asset, out info)) {
                Debug.LogErrorFormat("Not Found Bundle for:{0}", asset);
            }
            return info;
        }

        public static string PatchDir {
            get { return Path.Combine(Application.persistentDataPath, "Patch"); }
        }

        private static string _defaultDir;
        public static string DefaultDir {
            get {
                if (_defaultDir == null) {
#if UNITY_IOS
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
            _cacheBundles.Clear();
            _cacheAssets.Clear();
            _cacheRes.Clear();
            _asset2BundleInfoT.Clear();
            _bundleEntryC.Clear();

            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();
        }

        // 加载资源表
        public void LoadAssetsTable()
        {
            // 1. read current version
            var verPath = Path.Combine(DefaultDir, VERSION_FILE);
            OriginVersion = AppVersion.Parse(File.ReadAllText(verPath));
            verPath = Path.Combine(PatchDir, VERSION_FILE);
            if (File.Exists(verPath)) {
                CurrentVersion = AppVersion.Parse(File.ReadAllText(verPath));
            }
            else {
                CurrentVersion = OriginVersion;
            }

            // 2. 所有资源信息
            var defaultPath = Path.Combine(DefaultDir, ASSETS_FILE);
            var defaultAssets = LitJson.JsonMapper.ToObject<Dictionary<string, BundleEntry>>(File.ReadAllText(defaultPath));

            Dictionary<string, BundleEntry> patchBundle = null;
            string patchPath = Path.Combine(PatchDir, ASSETS_FILE + CurrentVersion.ToString());
            if (!File.Exists(patchPath)) {
                patchBundle = defaultAssets;
            }
            else {
                patchBundle = LitJson.JsonMapper.ToObject<Dictionary<string, BundleEntry>>(File.ReadAllText(patchPath));
            }
            BundleEntry defaultConf;
            foreach (var it in patchBundle) {
                var name = it.Key;
                var conf = it.Value;
                bool isPatch = true;
                if (defaultAssets.TryGetValue(name, out defaultConf) && defaultConf.Version == conf.Version) {
                    isPatch = false;
                }
                var bundle = new BundleInfo(name, conf, isPatch);
                _bundleEntryC.Add(name, bundle);
                var assetsCount = conf.Assets != null ? conf.Assets.Length : 0; ;
                for (int i = 0; i < assetsCount; ++i) {
                    _asset2BundleInfoT[conf.Assets[i]] = bundle;
                }
            }
        }
        private Coroutine LoadAssetManifest()
        {
            return LoadAssetAsync("AssetBundleManifest", (asset) => {
                _manifest = asset as AssetBundleManifest;
            });
        }
        #endregion

        private Coroutine LoadAssetAsync(string assetName, System.Action<Object> callback)
        {
            AsyncInfo info;
            if (_cacheAssets.TryGetValue(assetName, out info)) {
                if (info.State == AsyncState.Finish) {
                    callback?.Invoke(info.Asset);
                    return null;
                }
                else {
                    info.Callback += callback;
                    _cacheAssets[assetName] = info;
                    return info.Coroutine;
                }
            }
            BundleInfo entry;
            if (!_asset2BundleInfoT.TryGetValue(assetName, out entry)) {
                Debug.LogErrorFormat("Not Found Bundle for:{0}", assetName);
                return null;
            }
            info = new AsyncInfo() {
                State = AsyncState.Processing,
                Callback = callback
            };
            _cacheAssets.Add(assetName, info);

            var coroutine = MonoProxy.Instance.StartCoroutine(_loadAssetCoroutine(entry, assetName));
            if (coroutine != null) {
                info.Coroutine = coroutine; // 记录下，在重复load时再次返回这个协程
                _cacheAssets[assetName] = info;
            }
            return coroutine;
        }

        private IEnumerator _loadAssetCoroutine(BundleInfo bundleEntry, string assetName) {
            var coroutine = LoadBundleAsync(bundleEntry, null);
            if (coroutine != null) {
                yield return coroutine;
            }
            AsyncInfo info;
            AssetBundle bundle = null;
            if (_cacheBundles.TryGetValue(bundleEntry.Name, out info)) {
                if (info.State == AsyncState.Finish) {
                    bundle = info.Asset as AssetBundle;
                }
            }
            if (null == bundle) {
                Debug.LogErrorFormat("Error AssetBundles not loaded:{0}", bundleEntry.Name);
                yield break;
            }
            var assetReq = bundle.LoadAssetAsync(assetName);
            yield return assetReq;
            var asset = assetReq.asset;
            if (asset == null) {
                Debug.LogErrorFormat("Failed to load asset:{0}", assetName);
            }
            if (_cacheAssets.TryGetValue(assetName, out info)) {
                _cacheAssets[assetName] = new AsyncInfo() {
                    State = AsyncState.Finish,
                    Asset = asset
                };
                info.Callback?.Invoke(asset);
            }
        }

        private Coroutine LoadBundleAsync(string bundleName, System.Action<Object> handle)
        {
            BundleInfo entry;
            if (!_bundleEntryC.TryGetValue(bundleName, out entry)) {
                Debug.LogErrorFormat("Not Found AssetBundles:{0}", bundleName);
                return null;
            }
            return LoadBundleAsync(entry, handle);
        }

        /// <summary>
        /// 加载的协程，如果返回Coroutine的完成
        /// </summary>
        public Coroutine LoadBundleAsync(BundleInfo bundleEntry, System.Action<Object> callback)
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
            if (_cacheBundles.TryGetValue(name, out info)) {
                if (info.State == AsyncState.Finish) {
                    callback?.Invoke(info.Asset);
                    return null;
                }
                else {
                    info.Callback += callback;
                    _cacheBundles[name] = info;
                    return info.Coroutine;
                }
            }
            info = new AsyncInfo() {
                State = AsyncState.Processing,
                Callback = callback
            };
            _cacheBundles.Add(name, info);
            var coroutine = MonoProxy.Instance.StartCoroutine(_loadBundleCoroutine(bundleEntry));
            if (coroutine != null) {
                info.Coroutine = coroutine; // 记录下，在重复load时再次返回这个协程
                _cacheAssets[name] = info;
            }
            return coroutine;
        }

        private IEnumerator _loadBundleCoroutine(BundleInfo entry)
        {
            var name = entry.Name;
            if (_manifest != null) {
                string[] dependencies = _manifest.GetAllDependencies(name);
                var c = dependencies.Length;
                for (var i = 0; i < c; ++i) {
                    yield return LoadBundleAsync(dependencies[i], null);
                }
            }
            var req = AssetBundle.LoadFromFileAsync(entry.Url);
            yield return req;
            var bundle = req.assetBundle;
            if (bundle == null) {
                Debug.LogErrorFormat("Failed to load asset bundle:{0}", entry);
            }
            AsyncInfo info;
            if (_cacheBundles.TryGetValue(name, out info)) {
                _cacheBundles[name] = new AsyncInfo() {
                    State = AsyncState.Finish,
                    Asset = bundle
                };
                info.Callback?.Invoke(bundle);
            }
        }

        private Coroutine LoadResourceAsync(string assetName, System.Action<Object> callback)
        {
            //var assetName = assetName;
            AsyncInfo info;
            if (_cacheRes.TryGetValue(assetName, out info)) {
                if (info.State == AsyncState.Finish) {
                    callback?.Invoke(info.Asset);
                    return null;
                }
                else {
                    info.Callback += callback;
                    _cacheRes[assetName] = info;
                    return info.Coroutine;
                }
            }
            info = new AsyncInfo() {
                State = AsyncState.Processing,
                Callback = callback
            };
            _cacheAssets.Add(assetName, info);
            info.Coroutine = MonoProxy.Instance.StartCoroutine(_loadResCoroutine(assetName));
            if (info.Coroutine != null) {
                _cacheAssets[assetName] = info;
            }
            return info.Coroutine;
        }
        private IEnumerator _loadResCoroutine(string assetName)
        {
            var req = Resources.LoadAsync(assetName);
            yield return req;
            if (_cacheAssets.TryGetValue(assetName, out AsyncInfo info)) {
                _cacheAssets[assetName] = new AsyncInfo() {
                    State = AsyncState.Finish,
                    Asset = req.asset
                };
                info.Callback?.Invoke(req.asset);
            }
        }

        private Dictionary<string, string> _resFullPaths;
        private Dictionary<string, string> ResFullPaths {
            get {
                if (_resFullPaths == null) {
                    _resFullPaths = new Dictionary<string, string>();
                    var resDir = "Assets/Resources/";
                    var baseLen = resDir.Length;
                    var dir = resDir;
                    var files = Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories);
                    foreach (var file in files) {
                        var ext = Path.GetExtension(file);
                        if (ext.Equals(".meta")) {
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
        public Coroutine LoadAsync(string path, System.Action<Object> handle)
        {
#if UNITY_EDITOR
            Debug.LogFormat("Begin Load Asset:{0}", path);
            var begin = Time.time;
            handle += (obj) => {
                Debug.LogFormat("End Load Asset:{0}, Cost Time:{1}", path, Time.time - begin);
            };
#endif
            if (AppSetting.Instance.LoadInResource) {
                path = ResFullPaths[path];
                return LoadResourceAsync(path, handle);
            }
            return LoadAssetAsync(path, handle);
        }

    }

}

