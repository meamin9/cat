using System.IO;
using UnityEngine;

public partial class Asset
{
    public void LoadPrefab(string prefabName, System.Action<Object> handle=null) {
        if (AppConfig.Instance.IsResMode) {
            var path = Path.Combine("Prefabs", prefabName);
            LoadResource(path, handle);
            return;
        }
        LoadBundleAsset("assetBundle", prefabName, handle);
    }

    public void LoadAsset(string path, System.Action<Object> handle) {
        if (AppConfig.Instance.IsResMode) {
            LoadResource(path, handle);
            return;
        }
        LoadBundleAsset("assetBundle", path, handle);
    }

    public void LoadCfg(string path, System.Action<Object> handle) {
        if (AppConfig.Instance.IsResMode) {
            LoadResource(path, handle);
            return;
        }
        LoadBundleAsset("assetBundle", path, handle); 
    }
}
