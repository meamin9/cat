using System.Collections;
using UnityEngine;
using NSUnityUtil;
using System.IO;
using System.Collections.Generic;

public partial class Asset : SingleMonoBehaviour<Asset>
{
    Dictionary<string, AssetBundle> _cacheBundles = new Dictionary<string, AssetBundle>();
    Dictionary<string, Object> _cacheAssets = new Dictionary<string, Object>();
    Dictionary<string, System.Action<Object>> _assetHandles = new Dictionary<string, System.Action<Object>>();
    Dictionary<string, System.Action<AssetBundle>> _bundleHandles = new Dictionary<string, System.Action<AssetBundle>>();
    Dictionary<string, Object> _cacheResAsset = new Dictionary<string, Object>();
    Dictionary<string, System.Action<Object>> _resHandles = new Dictionary<string, System.Action<Object>>();

    private bool _TryGetAsset<T>(Dictionary<string, T> cache, Dictionary<string, System.Action<T>> handles,
                                 string key, System.Action<T> handle) where T: class {
        T obj;
        if (cache.TryGetValue(key, out obj)) {
            if (handle != null) {
                if (obj != null) {
                    handle(obj);
                }
                else {
                    handles[key] += handle;
                }   
            }
            return true;
        }
        cache[key] = null;
        if (handle != null) {
            handles[key] = handle; 
        }
        return false;
    }

    private void _LoadFinished<T>(Dictionary<string, T> cache, Dictionary<string, System.Action<T>> handles,
                                  string key, T asset) where T: class {
        System.Action<T> handle;
        if (handles.TryGetValue(key, out handle)) {
            handles.Remove(key);
            cache[key] = asset; // 是否缓存
            handle(asset);
        }
    }

    public void LoadResource(string assetName, System.Action<Object> handle) {
        var path = assetName; //Path.Combine(Application.streamingAssetsPath, assetName);
        if (!_TryGetAsset(_cacheResAsset, _resHandles, path, handle)) {
            StartCoroutine(_loadResCoroutine(path));
        }
    }

    public void LoadBundleAsset(string bundleName, string assetName, System.Action<Object> handle) {
        var bundlePath = Path.Combine(Application.streamingAssetsPath, bundleName);
        var assetKey = string.Format("{0}@{1}", bundleName, assetName);
        Object obj;
        if (_cacheAssets.TryGetValue(assetKey, out obj)) {
            if (obj != null) {
                handle(_cacheAssets[assetKey]);
            } else {
                _assetHandles[assetKey] += handle;
            }
            return;
        }
        _cacheAssets[assetKey] = null; // 占位，开始加载
        _assetHandles[assetKey] = handle; // 使用后会清掉
        LoadBundle(bundlePath, (bundle) => {
            StartCoroutine(_loadAssetFromBundleCoroutine(bundle, assetName, assetKey));
        });
    }

    private void LoadBundle(string bundlePath, System.Action<AssetBundle> handle) {
        AssetBundle bundle;
        if (_cacheBundles.TryGetValue(bundlePath, out bundle)) {
            if (bundle != null) {
                handle(bundle);
            }
            else {
                _bundleHandles[bundlePath] += handle;
            }
            return;
        }
        _cacheBundles[bundlePath] = null; // 占位，开始加载
        _bundleHandles[bundlePath] = handle; // 使用后会清掉
        StartCoroutine(_loadBundleCoroutine(bundlePath));
    }

    private IEnumerator _loadBundleCoroutine(string path) {
        var req = AssetBundle.LoadFromFileAsync(path);
        yield return req;
        var bundle = req.assetBundle;
        if (bundle == null) {
            Log.Errorf("Failed to load asset bundle:{0}", path);
        }
        System.Action<AssetBundle> handle;
        if (_bundleHandles.TryGetValue(path, out handle)) {
            _bundleHandles.Remove(path);
            _cacheBundles[path] = bundle; // 是否缓存
            handle(bundle);
        }
    }

    private IEnumerator _loadAssetFromBundleCoroutine(AssetBundle bundle, string assetName, string assetKey) {
        Object asset = null;
        if (bundle != null) {
            var req = bundle.LoadAssetAsync(assetName);
            yield return req;
            asset = req.asset;
            if (asset == null) {
                Log.Errorf("Failed to load asset:{0}", assetKey);
            }
        }
        System.Action<Object> handle;
        if (_assetHandles.TryGetValue(assetKey, out handle)) {
            _assetHandles.Remove(assetKey);
            _cacheAssets[assetKey] = asset; // 是否缓存
            handle(asset);
        }
    }

    private IEnumerator _loadResCoroutine(string assetPath) {
        var req = Resources.LoadAsync(assetPath);
        yield return req;
        var asset = req.asset;
        _LoadFinished(_cacheResAsset, _resHandles, assetPath, asset);
    }
}
