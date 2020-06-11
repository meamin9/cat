using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Base;

namespace Game
{
    public static class SceneManager
    {
        public static event System.Action OnSceneLoaded;

        private static SceneObject _curScene;

        public static SceneObject CurrentScene { get => _curScene; }

        /// <summary>
        /// 加载队列，第1个是正在加载的
        /// </summary>
        private static List<SceneObject> _loadingScenes = new List<SceneObject>();

        public static Coroutine SwitchScene(int sceneId)
        {
            if (_curScene != null && _curScene.Conf.Id == sceneId) {
                return null;
            }
            var destScene = SceneObject.CreateScene(sceneId);
            if (destScene == null) {
                Log.Error($"create scene error:{sceneId}");
                return null;
            }
            var count = _loadingScenes.Count;
            var prevScene = count > 0 ? _loadingScenes[count - 1] : _curScene;
            if (prevScene != null && !prevScene.CanSwitchTo(destScene)) {
                return null;
            }
            _loadingScenes.Add(destScene);
            if (count == 0) {
                return MonoProxy.Instance.StartCoroutine(_SwitchSceneCoroutine(destScene));
            }
            return null;
        }

        private static void _EndSwitchScene()
        {
            var scene = _loadingScenes[0];
            _loadingScenes.RemoveAt(0);
            if (_loadingScenes.Count > 0) {
                MonoProxy.Instance.StartCoroutine(_SwitchSceneCoroutine(_loadingScenes[0]));
                return;
            }
            
            OnSceneLoaded?.Invoke();
        }

        /// <summary>
        /// 是否有场景等待加载
        /// </summary>
        private static bool hasWaitLoadScene()
        {
            return _loadingScenes.Count > 1;
        }

        private static IEnumerator _SwitchSceneCoroutine(SceneObject scene)
        {
            // 加载过程中如果有新的场景加载任务进来，快速结束当前任务，加载新的场景
            if (hasWaitLoadScene()) {
                _EndSwitchScene();
                yield break;
            }
            var sceneAsset = scene.Conf.Asset;
            if (_curScene != null) {
                _curScene.OnLeave();
                if (_curScene.Conf.Asset == sceneAsset) { // 同样的资源
                    _EndSwitchScene();
                    yield break;
                }
                yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneAsset);
                _curScene = null;
                if (hasWaitLoadScene()) {
                    _EndSwitchScene();
                    yield break;
                }
            }

            var bundleInfo = AssetMgr.Instance.GetBundleInfo(sceneAsset);
            yield return AssetMgr.Instance.LoadBundleAsync(bundleInfo, null);
            if (hasWaitLoadScene()) {
                //TODO: unload bundle
                _EndSwitchScene();
                yield break;
            }
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneAsset, LoadSceneMode.Additive);
            if (hasWaitLoadScene()) {
                yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneAsset);
                _EndSwitchScene();
                yield break;
            }
#if UNITY_EDITOR
            GameObject mapRoot = GameObject.Find("MapRoot");
            if (mapRoot != null) {
                Renderer[] renders = mapRoot.GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < renders.Length; i++) {
                    renders[i].material.shader = Shader.Find(renders[i].material.shader.name);
                }
            }
#endif
            _curScene = scene;
            _curScene.OnEnter();
            _EndSwitchScene();
        }
    }
}
