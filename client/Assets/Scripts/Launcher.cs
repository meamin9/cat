﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    private const string VERSION_FILE = "Version";
    public const string ASSETS_FILE = "Bundles.json";
    public static string PatchDir
    {
        get { return Path.Combine(Application.persistentDataPath, "Patch"); }
    }

    public static string DefaultDir
    {
        get { return Application.streamingAssetsPath; }
    }

    private void Start()
    {
        StartCoroutine(LoadAssembly());
    }

    [System.Serializable]
    public struct VersionCodeInfo
    {
        public int Version;
    }

    IEnumerator LoadAssembly()
    {
        Dictionary<string, VersionCodeInfo> versionCodes = null;
        var versionPath = Path.Combine(PatchDir, VERSION_FILE);
        if (File.Exists(versionPath))
        {
            var version = File.ReadAllText(versionPath).Trim();
            var filePath = Path.Combine(PatchDir, ASSETS_FILE + version.ToString());
            versionCodes = JsonUtility.FromJson<Dictionary<string, VersionCodeInfo>>(File.ReadAllText(filePath));
        }
        Assembly patch = null;
        string[] _dllArray = { "Automata.Adapter.bytes", "Automata.Base.bytes", "Automata.Patch.bytes" };
        var n = _dllArray.Length;
        for(var i =0; i < n; ++i)
        {
            var name = _dllArray[i];
            string path = Path.Combine(Application.streamingAssetsPath, name);
            if (versionCodes != null)
            {
                VersionCodeInfo info;
                if(versionCodes.TryGetValue(name, out info))
                {
                    var patchPath = Path.Combine(PatchDir, name + info.Version);
                    if(File.Exists(patchPath))
                    {
                        path = patchPath;
                    }
                }
            }
            var req = AssetBundle.LoadFromFileAsync(path + ".ab");
            yield return req;
            var ab = req.assetBundle;
            var abReq = ab.LoadAssetAsync(name);
            yield return abReq;
            patch = Assembly.Load((abReq.asset as TextAsset).bytes);
            yield return null;
        }
        AssetBundle.UnloadAllAssetBundles(true);
        Resources.UnloadUnusedAssets();
        var type = patch.GetType("Automata.Patch.PatchStage");
        var ins = Activator.CreateInstance(type);
        var method = type.GetMethod("Enter");
        method.Invoke(ins, null);
    }
}

