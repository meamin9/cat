﻿using System.Collections;
using System.Collections.Generic;
using AM.Base;
using UnityEngine;
using System;
using System.IO;
using System.Reflection;

namespace AM.Patch
{
    public class PatchStage
    {
        private const string PatchTempDir = "";
        private long _downloadedSize = 0;
        private int _downloadedCount = 0;
        private long _patchSize = 0;
        private int _patchFileCount = 0;
        private List<KeyValuePair<string, BundleEntry>> _diffs;
        private bool _needReboot = false;
        private string[] _rebootAssets = { "Automata.Adapter.bytes", "Automata.Base.bytes", "Automata.Patch.bytes" };
        private byte[] _patchInfo;
        private AppVersion _patchVersion;

        private PatchStage()
        {
            //UIMgr.RegistUICreator(PatchUI.Index, UIMgr.GenUICreator<PatchUI>());
        }

        public static void Enter()
        {
            var stage = new PatchStage();
            MonoProxy.Instance.StartCoroutine(stage.Start());
        }

        private IEnumerator Start()
        {
            yield return Base.AssetMgr.InitAsync();
            AppVersion version;
            var versionUrl = AppSetting.Instance.PatchUrl + AssetMgr.VERSION_FILE;
            using (var www = new WWW(versionUrl))
            {
                yield return www;
                version = AppVersion.Parse(www.text);
                if (AssetMgr.Instance.CurrentVersion < version)
                {
                    Exit();
                    yield break;
                }
            }
            Debug.LogFormat("有新版本，准备更新 {0}->{1}", AssetMgr.Instance.CurrentVersion, version);
            //不要依赖uimgr，这个会经常改动
            //UIManager.Show(PatchUI.Index);
            //var tempDir = Path.Combine(Application.temporaryCachePath, PatchTempDir);
            //if (!Directory.Exists(tempDir))
            //{
            //    Directory.CreateDirectory(tempDir);
            //}
            _patchVersion = version;
            var patchDir = AssetMgr.PatchDir;
            if (!Directory.Exists(patchDir))
            {
                Directory.CreateDirectory(patchDir);
            }
            Dictionary<string, BundleEntry> assets;
            var assetsUrl = AppSetting.Instance.PatchUrl + AssetMgr.ASSETS_FILE;
            using (var www = new WWW(assetsUrl))
            {
                yield return www;
                assets = JsonUtility.FromJson<Dictionary<string, BundleEntry>>(www.text);
                yield return null;
                _patchInfo = www.bytes;
                var filePath = Path.Combine(AssetMgr.PatchDir, AssetMgr.ASSETS_FILE + version.ToString());
                File.WriteAllBytes(filePath, www.bytes);
            }
            _diffs = new List<KeyValuePair<string, BundleEntry>>();
            long size = 0;
            foreach (var it in assets)
            {
                var info = AssetMgr.Instance.GetBundleInfo(it.Key);
                if (info == null || info.VersionCode < it.Value.Version)
                {
                    _diffs.Add(it);
                    size += it.Value.Size;
                    if (_needReboot == false)
                    {
                        for (var i = 0; i < _rebootAssets.Length; ++i)
                        {
                            if (_rebootAssets[i] == it.Key)
                            {
                                _needReboot = true;
                                break;
                            }
                        }
                    }
                }
            }
            var count = _diffs.Count;
            Debug.LogFormat("更新文件数 {0}, 大小 {1}KB", _diffs.Count, (float)size / 1024);
            if (count == 0)
            {
                yield break;
            }
            _patchFileCount = count;
            _patchSize = size;
            //_diffs = diffs;
            MonoProxy.Instance.StartCoroutine(DownloadAssets(_diffs, 0, count / 4));
            MonoProxy.Instance.StartCoroutine(DownloadAssets(_diffs, count / 4, count / 2));
            MonoProxy.Instance.StartCoroutine(DownloadAssets(_diffs, count / 2, count / 2 + count / 4));
            MonoProxy.Instance.StartCoroutine(DownloadAssets(_diffs, count / 2 + count / 4, count));
        }

        private IEnumerator DownloadAssets(List<KeyValuePair<string, BundleEntry>> assets, int beginIndex, int endIndex)
        {
            for (var i = beginIndex; i < endIndex; ++i)
            {
                var asset = assets[i];
                var url = AppSetting.Instance.PatchUrl + asset.Key;
                var info = asset.Value;
                var fileName = asset.Key + asset.Value.Version.ToString();
                var filePath = Path.Combine(AssetMgr.PatchDir, fileName);
                if (!File.Exists(filePath))
                {
                    using (var www = new WWW(url))
                    {
                        yield return www;
                        File.WriteAllBytes(filePath, www.bytes);
                    }
                }
                _downloadedSize += asset.Value.Size;
                ++_downloadedCount;
                if (_downloadedCount == _patchFileCount)
                {
                    FinishDownload();
                }
            }
        }

        private void FinishDownload()
        {
            var versionPath = Path.Combine(AssetMgr.PatchDir, AssetMgr.VERSION_FILE);
            File.WriteAllText(versionPath, _patchVersion.ToString());
            // 删除旧的文件
            var path = Path.Combine(AssetMgr.PatchDir, AssetMgr.ASSETS_FILE + AssetMgr.Instance.CurrentVersion.ToString());
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var count = _diffs.Count;
            for(var i = 0; i < count; ++i)
            {
                var diff = _diffs[i];
                var info = AssetMgr.Instance.GetBundleInfo(diff.Key);
                if (info != null && !info.InApp)
                {
                    path = Path.Combine(AssetMgr.PatchDir, diff.Key + info.VersionCode.ToString());
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }
            Debug.LogFormat("更新完成");
            if (_needReboot)
            {
                Debug.LogFormat("更新后重启");
                return;
            }
            Exit();
        }

        private void Exit()
        {
            AssetMgr.Instance.Clear();
            AssetMgr.Instance.LoadAssetsTable();
            AssetMgr.Instance.LoadAsync("AM.Game.bytes", (asset) => {
                var textAsset = asset as TextAsset;
                var game = Assembly.Load(textAsset.bytes);
                var type = game.GetType("AM.Game.GameStage");
                //var ins = Activator.CreateInstance(type);
                var method = type.GetMethod("Enter");
                method.Invoke(null, null);
            });
        }
    }
}
