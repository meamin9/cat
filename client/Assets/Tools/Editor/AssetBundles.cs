using System.IO;
using UnityEditor;
using UnityEngine;
using AM.Base;
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
        var count = 0;
        float total = paths.Length;
        EditorUtility.DisplayProgressBar("Set Bundles Name", dir, count / total);
        foreach (var path in paths)
        {
            var importer = AssetImporter.GetAtPath(path);
            var bundleName = path.Substring(baseLen).Replace("\\", "/") + ".ab";
            importer.assetBundleName = bundleName;
            EditorUtility.DisplayProgressBar("Set Bundles Name", string.Format("{0}({1}/{2})", dir, count, total), count++ / total);
        }

        dir = resDir;// + "Views/";
        paths = Directory.GetFiles(dir, "*.prefab", SearchOption.AllDirectories);
        count = 0;
        total = paths.Length;
        foreach (var path in paths)
        {
            var importer = AssetImporter.GetAtPath(path);
            var bundleName = path.Substring(baseLen).Replace("\\", "/") + ".ab";
            importer.assetBundleName = bundleName;
            EditorUtility.DisplayProgressBar("Set Bundles Name", string.Format("{0}({1}/{2})", dir, count, total), count++ / total);
        }

        dir = resDir;// + "Views/";
        paths = Directory.GetFiles(dir, "*.mat", SearchOption.AllDirectories);
        count = 0;
        total = paths.Length;
        foreach (var path in paths)
        {
            var importer = AssetImporter.GetAtPath(path);
            var bundleName = path.Substring(baseLen).Replace("\\", "/") + ".ab";
            importer.assetBundleName = bundleName;
            EditorUtility.DisplayProgressBar("Set Bundles Name", string.Format("{0}({1}/{2})", dir, count, total), count++ / total);
        }

        dir = resDir;// + "Views/";
        paths = Directory.GetFiles(dir, "*.jpg", SearchOption.AllDirectories);
        count = 0;
        total = paths.Length;
        foreach (var path in paths)
        {
            var importer = AssetImporter.GetAtPath(path);
            var bundleName = path.Substring(baseLen).Replace("\\", "/") + ".ab";
            importer.assetBundleName = bundleName;
            EditorUtility.DisplayProgressBar("Set Bundles Name", string.Format("{0}({1}/{2})", dir, count, total), count++ / total);
        }

        dir = resDir;// + "Views/";
        paths = Directory.GetFiles(dir, "*.png", SearchOption.AllDirectories);
        count = 0;
        total = paths.Length;
        foreach (var path in paths)
        {
            var importer = AssetImporter.GetAtPath(path);
            var bundleName = path.Substring(baseLen).Replace("\\", "/") + ".ab";
            importer.assetBundleName = bundleName;
            EditorUtility.DisplayProgressBar("Set Bundles Name", string.Format("{0}({1}/{2})", dir, count, total), count++ / total);
        }


        dir = resDir + "Config/";
        paths = Directory.GetFiles(dir, "*.asset", SearchOption.AllDirectories);
        count = 0;
        total = paths.Length;
        foreach (var path in paths)
        {
            var importer = AssetImporter.GetAtPath(path);
            var bundleName = path.Substring(baseLen).Replace("\\", "/") + ".ab";
            importer.assetBundleName = bundleName;
            EditorUtility.DisplayProgressBar("Set Bundles Name", string.Format("{0}({1}/{2})", dir, count, total), count++ / total);
        }

        dir = resDir + "Tables/";
        paths = Directory.GetFiles(dir, "*.json", SearchOption.AllDirectories);
        count = 0;
        total = paths.Length;
        foreach (var path in paths)
        {
            var importer = AssetImporter.GetAtPath(path);
            var bundleName = "tables.ab";
            importer.assetBundleName = bundleName;
            EditorUtility.DisplayProgressBar("Set Bundles Name", string.Format("{0}({1}/{2})", dir, count, total), count++ / total);
        }
        EditorUtility.ClearProgressBar();
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
        var assets = new Dictionary<string, BundleEntry>();
        if (File.Exists(infoPath))
        {
            assets = LitJson.JsonMapper.ToObject< Dictionary<string, BundleEntry>>(File.ReadAllText(infoPath));
        }
        var newAssets = new Dictionary<string, BundleEntry>();
        var paths = Directory.GetFiles(dir, "*.manifest", SearchOption.AllDirectories);
        var count = 0;
        float total = paths.Length + 1;
        foreach (var path in paths)
        {
            var filePath = path.Substring(0, path.Length - ".manifest".Length);
            var newConf = new BundleEntry();
            var fileInfo = new FileInfo(filePath);
            newConf.Size = fileInfo.Length;
            var name = filePath.Substring(prefixLen).Replace("\\", "/");
            newAssets.Add(name, newConf);
            BundleEntry conf;
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
                            //场景资源不加后缀名
                            assetName = Path.GetFileNameWithoutExtension(assetName);
                        }
                        assetList.Add(assetName);
                    }
                }
                newConf.Assets = assetList.ToArray();
            }
            EditorUtility.DisplayProgressBar("Gen Version Info", dir, ++count / total);
        }
        var content = LitJson.JsonMapper.ToJson(newAssets);
        File.WriteAllText(infoPath, content);
        EditorUtility.DisplayProgressBar("Gen Version Info", dir, ++count / total);
        EditorUtility.ClearProgressBar();
    }

    [System.Serializable]
    public struct VersionJson
    {
        public Dictionary<string, BundleEntry> A;
    }

}