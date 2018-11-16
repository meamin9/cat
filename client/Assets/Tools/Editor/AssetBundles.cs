using System.IO;
using UnityEditor;
using UnityEngine;
using Automata.Base;
using System.Collections.Generic;

public static class DevTools
{
    public static string AndriodAssetBundlesDir = "Assets/StreamingAssets/Android/AssetBundles/";
    static DevTools()
    {
        string[] paths = {
            AndriodAssetBundlesDir
        };
        foreach(var path in paths)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }

    [MenuItem("Tools/Build Android")]
    public static void BuildAssetBundlesAndroid()
    {
        SetAssetBundlesName();
        BuildAssetBundlesAndroidLZ4();
        BuildVersionForAndroid();
    }

    [MenuItem("Tools/AssetBundles/Build Android LZMA")]
    public static void BuildAssetBundlesAndroidLZMA()
    {
        BuildPipeline.BuildAssetBundles(AndriodAssetBundlesDir, BuildAssetBundleOptions.None, BuildTarget.Android);
    }

    [MenuItem("Tools/AssetBundles/Build Android LZ4")]
    public static void BuildAssetBundlesAndroidLZ4()
    {
        BuildPipeline.BuildAssetBundles(AndriodAssetBundlesDir, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
    }

    [MenuItem("Tools/AssetBundles/Set Bundles Name")]
    public static void SetAssetBundlesName()
    {
        var resDir = "Assets/Resources/";
        var baseLen = resDir.Length;
        var dir = resDir + "Scenes/";
        var paths = Directory.GetFiles(dir, "*.unity", SearchOption.AllDirectories);
        foreach(var path in paths)
        {
            var importer = AssetImporter.GetAtPath(path);
            var bundleName = path.Substring(baseLen).Replace("\\", "/") + ".ab";
            importer.assetBundleName = bundleName;
        }

        dir = resDir;// + "Views/";
        paths = Directory.GetFiles(dir, "*.prefab", SearchOption.AllDirectories);
        foreach (var path in paths)
        {
            var importer = AssetImporter.GetAtPath(path);
            var bundleName = path.Substring(baseLen).Replace("\\", "/") + ".ab";
            importer.assetBundleName = bundleName;
        }

        dir = resDir + "Config/";
        paths = Directory.GetFiles(dir, "*.asset", SearchOption.AllDirectories);
        foreach (var path in paths)
        {
            var importer = AssetImporter.GetAtPath(path);
            var bundleName = path.Substring(baseLen).Replace("\\", "/") + ".ab";
            importer.assetBundleName = bundleName;
        }

        dir = resDir + "Tables/";
        paths = Directory.GetFiles(dir, "*.json", SearchOption.AllDirectories);
        foreach (var path in paths)
        {
            var importer = AssetImporter.GetAtPath(path);
            var bundleName = "tables.ab";
            importer.assetBundleName = bundleName;
        }
    }

    [MenuItem("Tools/AssetBundles/Build Version Info (Android)")]
    public static void BuildVersionForAndroid()
    {
        BuildVersionInfo(AndriodAssetBundlesDir);
    }
    public static void BuildVersionInfo(string dir)
    {
        var infoPath = dir + AssetMgr.ASSETS_FILE;
        var prefixLen = dir.Length;
        var assets = new Dictionary<string, BundleConf>();
        if (File.Exists(infoPath))
        {
            assets = LitJson.JsonMapper.ToObject< Dictionary<string, BundleConf>>(File.ReadAllText(infoPath));
        }
        var newAssets = new Dictionary<string, BundleConf>();
        var paths = Directory.GetFiles(dir, "*.manifest", SearchOption.AllDirectories);
        foreach (var path in paths)
        {
            var filePath = path.Substring(0, path.Length - ".manifest".Length);
            var newConf = new BundleConf();
            var fileInfo = new FileInfo(filePath);
            newConf.Size = fileInfo.Length;
            var name = filePath.Substring(prefixLen).Replace("\\", "/");
            newAssets.Add(name, newConf);
            BundleConf conf;
            if(assets.TryGetValue(name, out conf))
            {
                newConf.Version = conf.Version;
            }
            if (name == "AssetBundles")
            {
                newConf.Assets = new string[] { "AssetBundleManifest" };
            }
            else {
                var assetList = new List<string>();
                foreach(var line in File.ReadLines(path))
                {
                    if (line.StartsWith("- Assets/Resources/"))
                    {
                        var assetName = Path.GetFileName(line);
                        if (assetName.EndsWith(".unity"))
                        {
                            //������Դ���Ӻ�׺��
                            assetName = Path.GetFileNameWithoutExtension(assetName);
                        }
                        assetList.Add(assetName);
                    }
                }
                newConf.Assets = assetList.ToArray();
            }
        }
        var content = LitJson.JsonMapper.ToJson(newAssets);
        File.WriteAllText(infoPath, content);
    }

    [System.Serializable]
    public struct VersionJson
    {
        public Dictionary<string, BundleConf> A;
    }

}