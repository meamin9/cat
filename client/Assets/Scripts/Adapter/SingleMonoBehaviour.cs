using UnityEngine;
using UnityEngine.SceneManagement;

namespace Base
{
    public class SingleMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var typ = typeof(T);
                    var obj = new GameObject(typ.Name);
                    _instance = obj.AddComponent(typ) as T;
                    //ntDestroyScene.Move(obj);
                }
                return _instance;
            }
        }
        public static void ReleaseInstance()
        {
            _instance = null;
        }
    }

    public static class DontDestroyScene
    {
        private static System.Nullable<Scene> _dontDestroyScene = null;
        public static Scene SceneInstance
        {
            get
            {
                if (!_dontDestroyScene.HasValue)
                {
                    _dontDestroyScene = SceneManager.CreateScene("#DontDestroyScene");
                }
                return _dontDestroyScene.Value;
            }
        }
        public static void Move(GameObject obj)
        {
            SceneManager.MoveGameObjectToScene(obj, SceneInstance);
        }

    }


    /// <summary>
    /// 全局的monoBehaviour
    /// </summary>
    public class MonoProxy : SingleMonoBehaviour<MonoProxy>
    {
        public MonoBehaviourAdapter Adapter { get; private set; }
        private void Awake()
        {
            Adapter = gameObject.AddComponent<MonoBehaviourAdapter>();
        }
    }

}
