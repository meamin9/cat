using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Automata.Base;
using Automata.Adapter;

namespace Automata.Game
{

    public class SceneObject
    {
        public SceneEntry Entry;

        public SceneObject(SceneEntry entry)
        {
            Entry = entry;
        }
    }

    public class SceneMgr : Singleton<SceneMgr>
    {

        private SceneObject _curScene;
        private SceneEntry _nextSceneEntry;


        public List<SceneEntry> _loadingSceneEntrys = new List<SceneEntry>();
        public void SwitchScene(int sceneId)
        {
            if (_curScene.Entry.Id == sceneId)
            {
                return;
            }
            SceneEntry entry = SceneEntry.Find(sceneId);
            if (entry == null)
            {
                Log.Errorf("Not Found Scene Entry:{0}", sceneId);
                return;
            }
            _loadingSceneEntrys.Add(entry);
            if (!IsWaitLoadScene())
            {
                MonoProxy.Instance.StartCoroutine(_SwitchSceneCoroutine(entry));
            }
        }

        private void _EndSwitchScene()
        {
            _loadingSceneEntrys.RemoveAt(0);
            if (_loadingSceneEntrys.Count > 0)
            {
                MonoProxy.Instance.StartCoroutine(_SwitchSceneCoroutine(_loadingSceneEntrys[0]));
            }
        }

        public bool IsLoadingScene()
        {
            return _loadingSceneEntrys.Count > 0;
        }

        public bool IsWaitLoadScene()
        {
            return _loadingSceneEntrys.Count > 1;
        }

        private IEnumerator _SwitchSceneCoroutine(SceneEntry entry)
        {
            if (IsWaitLoadScene())
            {
                _EndSwitchScene();
                yield break;
            }
            if (_curScene != null)
            {
                yield return SceneManager.UnloadSceneAsync(_curScene.Entry.Name);
                _curScene = null;
                if (IsWaitLoadScene())
                {
                    _EndSwitchScene();
                    yield break;
                }
            }

            var bundleInfo = AssetMgr.Instance.GetBundleEntry(entry.ResPath);
            yield return AssetMgr.Instance.LoadBundleAsync(bundleInfo, null);
            if (IsWaitLoadScene())
            {
                _EndSwitchScene();
                yield break;
            }
            yield return SceneManager.LoadSceneAsync(entry.Name, LoadSceneMode.Additive);
            if (IsWaitLoadScene())
            {
                yield return SceneManager.UnloadSceneAsync(entry.Name);
                _EndSwitchScene();
                yield break;
            }
            _EndSwitchScene();
        }
    }
}
